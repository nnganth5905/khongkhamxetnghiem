import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

// =====================================================
// NORMALIZE LIST
// =====================================================

function normalizeList(payload) {
  if (Array.isArray(payload)) {
    return payload;
  }

  if (Array.isArray(payload?.content)) {
    return payload.content;
  }

  if (Array.isArray(payload?.data)) {
    return payload.data;
  }

  if (Array.isArray(payload?.items)) {
    return payload.items;
  }

  if (Array.isArray(payload?.results)) {
    return payload.results;
  }

  return [];
}

// =====================================================
// COLUMN
// =====================================================

function getColumnKey(
  column,
  index,
) {
  if (
    typeof column ===
    'string'
  ) {
    return column;
  }

  return (
    column?.key ||
    column?.field ||
    column?.accessor ||
    column?.name ||
    `column-${index}`
  );
}

function getColumnLabel(
  column,
) {
  if (
    typeof column ===
    'string'
  ) {
    return column;
  }

  return (
    column?.label ||
    column?.title ||
    column?.header ||
    column?.name ||
    column?.key ||
    column?.field ||
    ''
  );
}

function resolveValue(
  row,
  column,
  index,
) {
  if (!column) {
    return '';
  }

  /*
   * FIX:
   * QuanLyKhachHang.jsx đang sử dụng:
   *
   * value: (row) => ...
   */
  if (
    typeof column?.value ===
    'function'
  ) {
    return column.value(row);
  }

  if (
    typeof column?.render ===
    'function'
  ) {
    return column.render(row);
  }

  const key =
    getColumnKey(
      column,
      index,
    );

  return row?.[key];
}

function renderValue(
  row,
  column,
  index,
) {
  const value =
    resolveValue(
      row,
      column,
      index,
    );

  if (
    column?.type ===
    'status'
  ) {
    const text =
      String(
        value ?? '',
      );

    const active =
      text
        .toLowerCase()
        .includes(
          'hoạt động',
        ) ||
      text
        .toLowerCase()
        .includes(
          'active',
        );

    return (
      <span
        className={
          active
            ? 'badge bg-success-subtle text-success'
            : 'badge bg-secondary-subtle text-secondary'
        }
      >
        {text || '—'}
      </span>
    );
  }

  if (
    value === null ||
    value === undefined ||
    value === ''
  ) {
    return '—';
  }

  return value;
}

// =====================================================
// FORM
// =====================================================

function getFieldName(field) {
  if (
    typeof field ===
    'string'
  ) {
    return field;
  }

  return (
    field?.name ||
    field?.key ||
    field?.field ||
    ''
  );
}

function makeInitialForm(
  fields,
  initialValues,
) {
  const form = {
    ...(initialValues || {}),
  };

  for (
    const field of
    fields || []
  ) {
    const key =
      getFieldName(
        field,
      );

    if (
      key &&
      form[key] ===
        undefined
    ) {
      form[key] =
        typeof field ===
        'object'
          ? (
              field
                ?.defaultValue ??
              ''
            )
          : '';
    }
  }

  return form;
}

function buildPayload(
  fields,
  form,
) {
  const payload = {};

  for (
    const field of
    fields || []
  ) {
    const key =
      getFieldName(
        field,
      );

    if (!key) {
      continue;
    }

    const config =
      typeof field ===
      'object'
        ? field
        : {};

    let value =
      form?.[key];

    /*
     * Rất quan trọng cho .NET:
     *
     * DateOnly? không nhận JSON "".
     * Field optional rỗng phải gửi null.
     */
    if (
      value === ''
      &&
      !config.required
    ) {
      value = null;
    }

    payload[key] =
      value;
  }

  return payload;
}

// =====================================================
// ERROR
// =====================================================

function getErrorMessage(
  error,
  fallback,
) {
  const validation =
    error?.response
      ?.data?.data;

  if (
    validation &&
    typeof validation ===
      'object'
  ) {
    const values =
      Object.values(
        validation,
      );

    if (
      values.length > 0
    ) {
      return String(
        values[0],
      );
    }
  }

  return (
    error?.response
      ?.data?.message ||
    error?.response
      ?.data?.error ||
    error?.message ||
    fallback
  );
}

// =====================================================
// COMPONENT
// =====================================================

export default function AdminCrudPage(
  props,
) {
  const {
    title =
      'Quản lý dữ liệu',

    columns = [],

    fields = [],

    idKey =
      'id',

    initialValues = {},

    searchPlaceholder =
      'Tìm kiếm...',
  } = props;

  /*
   * FIX:
   * QuanLyKhachHang dùng subtitle,
   * component cũ chỉ đọc description.
   */
  const description =
    props.description ||
    props.subtitle ||
    '';

  const addButtonText =
    props.addButtonText ||
    'Thêm mới';

  const formTitleCreate =
    props.formTitleCreate ||
    'Thêm mới';

  const formTitleEdit =
    props.formTitleEdit ||
    'Cập nhật';

  const service =
    props.service || {};

  /*
   * FIX QUAN TRỌNG:
   * thêm props.loadItems
   */
  const loadFn =
    props.loadItems ||
    props.loadData ||
    props.fetchData ||
    props.getData ||
    props.listData ||
    props.onLoad ||
    service.list ||
    service.getAll;

  const createFn =
    props.createItem ||
    props.createData ||
    props.onCreate ||
    service.create;

  const updateFn =
    props.updateItem ||
    props.updateData ||
    props.onUpdate ||
    service.update;

  const deleteFn =
    props.deleteItem ||
    props.deleteData ||
    props.onDelete ||
    service.remove ||
    service.delete;

  const [
    items,
    setItems,
  ] = useState([]);

  const [
    loading,
    setLoading,
  ] = useState(true);

  const [
    saving,
    setSaving,
  ] = useState(false);

  const [
    deletingId,
    setDeletingId,
  ] = useState(null);

  const [
    error,
    setError,
  ] = useState('');

  const [
    message,
    setMessage,
  ] = useState('');

  const [
    query,
    setQuery,
  ] = useState('');

  const [
    showForm,
    setShowForm,
  ] = useState(false);

  const [
    editingItem,
    setEditingItem,
  ] = useState(null);

  const [
    form,
    setForm,
  ] = useState(
    makeInitialForm(
      fields,
      initialValues,
    ),
  );

  // ===================================================
  // ITEM ID
  // ===================================================

  function getItemId(
    item,
  ) {
    if (
      typeof props.itemId ===
      'function'
    ) {
      return props.itemId(
        item,
      );
    }

    return (
      item?.[idKey] ??
      item?.id ??
      null
    );
  }

  // ===================================================
  // LOAD DATA
  // ===================================================

  async function reload() {
    setLoading(true);
    setError('');

    try {
      if (
        typeof loadFn !==
        'function'
      ) {
        throw new Error(
          'Trang chưa được cấu hình hàm tải dữ liệu.',
        );
      }

      const payload =
        await loadFn();

      const normalized =
        normalizeList(
          payload,
        );

      setItems(
        normalized,
      );
    } catch (err) {
      setItems([]);

      setError(
        getErrorMessage(
          err,
          'Không thể tải dữ liệu.',
        ),
      );
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    reload();

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // ===================================================
  // SEARCH
  // ===================================================

  const visibleItems =
    useMemo(() => {
      const keyword =
        query
          .trim()
          .toLowerCase();

      if (!keyword) {
        return items;
      }

      return items.filter(
        (item) => {
          /*
           * Search trên dữ liệu gốc.
           */
          const directMatch =
            Object
              .values(
                item || {},
              )
              .some(
                (value) =>
                  String(
                    value ?? '',
                  )
                    .toLowerCase()
                    .includes(
                      keyword,
                    ),
              );

          if (
            directMatch
          ) {
            return true;
          }

          /*
           * Search cả các giá trị được tính
           * bởi column.value().
           */
          return columns.some(
            (
              column,
              index,
            ) =>
              String(
                resolveValue(
                  item,
                  column,
                  index,
                ) ?? '',
              )
                .toLowerCase()
                .includes(
                  keyword,
                ),
          );
        },
      );
    }, [
      items,
      query,
      columns,
    ]);

  // ===================================================
  // CREATE
  // ===================================================

  function openCreate() {
    setEditingItem(
      null,
    );

    setForm(
      makeInitialForm(
        fields,
        initialValues,
      ),
    );

    setShowForm(
      true,
    );

    setError('');
    setMessage('');
  }

  // ===================================================
  // EDIT
  // ===================================================

  function openEdit(
    item,
  ) {
    setEditingItem(
      item,
    );

    const initial =
      makeInitialForm(
        fields,
        initialValues,
      );

    /*
     * Chỉ đưa các field cần chỉnh sửa
     * vào form.
     */
    for (
      const field of
      fields || []
    ) {
      const name =
        getFieldName(
          field,
        );

      if (!name) {
        continue;
      }

      if (
        item?.[name] !==
        undefined
      ) {
        initial[name] =
          item[name] ??
          '';
      }
    }

    setForm(
      initial,
    );

    setShowForm(
      true,
    );

    setError('');
    setMessage('');
  }

  // ===================================================
  // CLOSE
  // ===================================================

  function closeForm() {
    if (saving) {
      return;
    }

    setShowForm(
      false,
    );

    setEditingItem(
      null,
    );

    setForm(
      makeInitialForm(
        fields,
        initialValues,
      ),
    );
  }

  // ===================================================
  // UPDATE FORM
  // ===================================================

  function updateForm(
    key,
    value,
  ) {
    setForm(
      (current) => ({
        ...current,
        [key]: value,
      }),
    );
  }

  // ===================================================
  // SUBMIT
  // ===================================================

  async function handleSubmit(
    event,
  ) {
    event.preventDefault();

    setError('');
    setMessage('');
    setSaving(true);

    try {
      const payload =
        buildPayload(
          fields,
          form,
        );

      if (
        editingItem
      ) {
        if (
          typeof updateFn !==
          'function'
        ) {
          throw new Error(
            'Trang chưa được cấu hình hàm cập nhật.',
          );
        }

        const id =
          getItemId(
            editingItem,
          );

        if (
          id === null ||
          id === undefined ||
          id === ''
        ) {
          throw new Error(
            'Không xác định được ID bản ghi cần cập nhật.',
          );
        }

        if (
          props.onUpdate &&
          !props.updateData &&
          !props.updateItem
        ) {
          await updateFn(
            payload,
            editingItem,
          );
        } else {
          await updateFn(
            id,
            payload,
          );
        }

        setMessage(
          'Cập nhật thành công.',
        );
      } else {
        if (
          typeof createFn !==
          'function'
        ) {
          throw new Error(
            'Trang chưa được cấu hình hàm thêm mới.',
          );
        }

        await createFn(
          payload,
        );

        setMessage(
          'Thêm mới thành công.',
        );
      }

      setShowForm(
        false,
      );

      setEditingItem(
        null,
      );

      setForm(
        makeInitialForm(
          fields,
          initialValues,
        ),
      );

      await reload();
    } catch (err) {
      setError(
        getErrorMessage(
          err,
          'Không thể lưu dữ liệu.',
        ),
      );
    } finally {
      setSaving(false);
    }
  }

  // ===================================================
  // DELETE
  // ===================================================

  async function handleDelete(
    item,
  ) {
    if (
      typeof deleteFn !==
      'function'
    ) {
      setError(
        'Trang chưa được cấu hình hàm xóa.',
      );

      return;
    }

    const id =
      getItemId(
        item,
      );

    if (
      id === null ||
      id === undefined ||
      id === ''
    ) {
      setError(
        'Không xác định được ID bản ghi.',
      );

      return;
    }

    const ok =
      window.confirm(
        'Bạn có chắc muốn ngừng hoạt động bản ghi này?',
      );

    if (!ok) {
      return;
    }

    setError('');
    setMessage('');

    setDeletingId(
      id,
    );

    try {
      if (
        props.onDelete &&
        !props.deleteData &&
        !props.deleteItem
      ) {
        await deleteFn(
          item,
          id,
        );
      } else {
        await deleteFn(
          id,
        );
      }

      setMessage(
        'Cập nhật trạng thái thành công.',
      );

      await reload();
    } catch (err) {
      setError(
        getErrorMessage(
          err,
          'Không thể cập nhật trạng thái.',
        ),
      );
    } finally {
      setDeletingId(
        null,
      );
    }
  }

  // ===================================================
  // COLUMNS
  // ===================================================

  const actualColumns =
    columns.length > 0
      ? columns
      : (
          items[0]
            ? Object
                .keys(
                  items[0],
                )
                .slice(
                  0,
                  6,
                )
            : []
        );

  // ===================================================
  // UI
  // ===================================================

  return (
    <div className="container-fluid py-4">

      {/* HEADER */}
      <div className="d-flex flex-wrap align-items-center justify-content-between gap-3 mb-4">
        <div>
          <h2 className="fw-bold mb-1">
            {title}
          </h2>

          {description && (
            <p className="text-muted mb-0">
              {description}
            </p>
          )}
        </div>

        <button
          type="button"
          className="btn btn-primary"
          onClick={
            openCreate
          }
          disabled={
            typeof createFn !==
            'function'
          }
        >
          <i className="fa-solid fa-plus me-2" />

          {addButtonText}
        </button>
      </div>

      {/* MESSAGE */}
      {message && (
        <div className="alert alert-success">
          {message}
        </div>
      )}

      {/* ERROR */}
      {error && (
        <div className="alert alert-danger">
          {error}
        </div>
      )}

      {/* TABLE */}
      <div className="card border-0 shadow-sm">
        <div className="card-body">

          {/* SEARCH */}
          <div className="mb-3">
            <input
              type="search"
              className="form-control"
              placeholder={
                searchPlaceholder
              }
              value={
                query
              }
              onChange={
                (event) =>
                  setQuery(
                    event.target.value,
                  )
              }
            />
          </div>

          {loading ? (
            <div className="text-center py-5">
              <div
                className="spinner-border text-primary"
                role="status"
              />

              <div className="text-muted mt-2">
                Đang tải dữ liệu...
              </div>
            </div>
          ) : (
            <div className="table-responsive">
              <table className="table table-hover align-middle">

                <thead className="table-light">
                  <tr>
                    {actualColumns.map(
                      (
                        column,
                        index,
                      ) => (
                        <th
                          key={
                            getColumnKey(
                              column,
                              index,
                            )
                          }
                        >
                          {
                            getColumnLabel(
                              column,
                            )
                          }
                        </th>
                      ),
                    )}

                    <th
                      className="text-end"
                      style={{
                        minWidth:
                          145,
                      }}
                    >
                      Thao tác
                    </th>
                  </tr>
                </thead>

                <tbody>
                  {visibleItems.length ===
                  0 ? (
                    <tr>
                      <td
                        colSpan={
                          actualColumns.length +
                          1
                        }
                        className="text-center text-muted py-5"
                      >
                        Chưa có dữ liệu.
                      </td>
                    </tr>
                  ) : (
                    visibleItems.map(
                      (
                        item,
                        rowIndex,
                      ) => {
                        const rowId =
                          getItemId(
                            item,
                          );

                        return (
                          <tr
                            key={
                              String(
                                rowId ??
                                rowIndex,
                              )
                            }
                          >
                            {actualColumns.map(
                              (
                                column,
                                columnIndex,
                              ) => (
                                <td
                                  key={
                                    getColumnKey(
                                      column,
                                      columnIndex,
                                    )
                                  }
                                >
                                  {
                                    renderValue(
                                      item,
                                      column,
                                      columnIndex,
                                    )
                                  }
                                </td>
                              ),
                            )}

                            <td className="text-end">

                              <button
                                type="button"
                                className="btn btn-sm btn-outline-primary me-2"
                                onClick={() =>
                                  openEdit(
                                    item,
                                  )
                                }
                                disabled={
                                  typeof updateFn !==
                                  'function'
                                }
                              >
                                <i className="fa-solid fa-pen me-1" />
                                Sửa
                              </button>

                              <button
                                type="button"
                                className="btn btn-sm btn-outline-danger"
                                onClick={() =>
                                  handleDelete(
                                    item,
                                  )
                                }
                                disabled={
                                  typeof deleteFn !==
                                    'function'
                                  ||
                                  deletingId ===
                                    rowId
                                }
                              >
                                {deletingId ===
                                rowId ? (
                                  <>
                                    <span className="spinner-border spinner-border-sm me-1" />
                                    Đang xử lý
                                  </>
                                ) : (
                                  <>
                                    <i className="fa-solid fa-ban me-1" />
                                    Khóa
                                  </>
                                )}
                              </button>

                            </td>
                          </tr>
                        );
                      },
                    )
                  )}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>

      {/* MODAL */}
      {showForm && (
        <div
          className="modal d-block"
          tabIndex="-1"
          style={{
            background:
              'rgba(15, 23, 42, 0.48)',
          }}
        >
          <div className="modal-dialog modal-lg modal-dialog-scrollable">
            <div className="modal-content">

              <form
                onSubmit={
                  handleSubmit
                }
              >

                {/* MODAL HEADER */}
                <div className="modal-header">
                  <h5 className="modal-title">
                    {editingItem
                      ? formTitleEdit
                      : formTitleCreate}
                  </h5>

                  <button
                    type="button"
                    className="btn-close"
                    onClick={
                      closeForm
                    }
                    disabled={
                      saving
                    }
                  />
                </div>

                {/* BODY */}
                <div className="modal-body">

                  {fields.length ===
                  0 ? (
                    <div className="alert alert-info mb-0">
                      Trang chưa khai báo danh sách trường nhập liệu.
                    </div>
                  ) : (
                    <div className="row g-3">

                      {fields.map(
                        (
                          field,
                          index,
                        ) => {
                          const config =
                            typeof field ===
                            'string'
                              ? {
                                  name:
                                    field,

                                  label:
                                    field,
                                }
                              : field;

                          const name =
                            getFieldName(
                              config,
                            );

                          if (!name) {
                            return null;
                          }

                          const type =
                            config.type ||
                            'text';

                          const label =
                            config.label ||
                            config.title ||
                            name;

                          const options =
                            config.options ||
                            [];

                          /*
                           * FIX:
                           * QuanLyKhachHang dùng colClass
                           * nhưng component cũ chỉ đọc className.
                           */
                          const colClass =
                            config.colClass ||
                            config.className ||
                            'col-12 col-md-6';

                          return (
                            <div
                              className={
                                colClass
                              }
                              key={
                                name ||
                                index
                              }
                            >
                              <label className="form-label">
                                {label}

                                {config.required &&
                                  (
                                    <span className="text-danger">
                                      {' *'}
                                    </span>
                                  )}
                              </label>

                              {type ===
                              'select' ? (
                                <select
                                  className="form-select"
                                  value={
                                    form[
                                      name
                                    ] ??
                                    ''
                                  }
                                  required={
                                    Boolean(
                                      config.required,
                                    )
                                  }
                                  onChange={
                                    (event) =>
                                      updateForm(
                                        name,
                                        event
                                          .target
                                          .value,
                                      )
                                  }
                                >
                                  <option value="">
                                    -- Chọn --
                                  </option>

                                  {options.map(
                                    (
                                      option,
                                      optionIndex,
                                    ) => {
                                      const value =
                                        typeof option ===
                                        'object'
                                          ? (
                                              option.value ??
                                              option.id ??
                                              option.key
                                            )
                                          : option;

                                      const optionLabel =
                                        typeof option ===
                                        'object'
                                          ? (
                                              option.label ??
                                              option.name ??
                                              option.title ??
                                              value
                                            )
                                          : option;

                                      return (
                                        <option
                                          value={
                                            value
                                          }
                                          key={
                                            String(
                                              value ??
                                              optionIndex,
                                            )
                                          }
                                        >
                                          {
                                            optionLabel
                                          }
                                        </option>
                                      );
                                    },
                                  )}
                                </select>
                              ) : type ===
                                'textarea' ? (
                                <textarea
                                  className="form-control"
                                  rows={
                                    config.rows ||
                                    4
                                  }
                                  value={
                                    form[
                                      name
                                    ] ??
                                    ''
                                  }
                                  required={
                                    Boolean(
                                      config.required,
                                    )
                                  }
                                  placeholder={
                                    config.placeholder ||
                                    ''
                                  }
                                  onChange={
                                    (event) =>
                                      updateForm(
                                        name,
                                        event
                                          .target
                                          .value,
                                      )
                                  }
                                />
                              ) : (
                                <input
                                  type={
                                    type
                                  }
                                  className="form-control"
                                  value={
                                    form[
                                      name
                                    ] ??
                                    ''
                                  }
                                  required={
                                    Boolean(
                                      config.required,
                                    )
                                  }
                                  placeholder={
                                    config.placeholder ||
                                    ''
                                  }
                                  min={
                                    config.min
                                  }
                                  max={
                                    config.max
                                  }
                                  step={
                                    config.step
                                  }
                                  onChange={
                                    (event) =>
                                      updateForm(
                                        name,
                                        event
                                          .target
                                          .value,
                                      )
                                  }
                                />
                              )}
                            </div>
                          );
                        },
                      )}

                    </div>
                  )}

                </div>

                {/* FOOTER */}
                <div className="modal-footer">

                  <button
                    type="button"
                    className="btn btn-light"
                    onClick={
                      closeForm
                    }
                    disabled={
                      saving
                    }
                  >
                    Hủy
                  </button>

                  <button
                    type="submit"
                    className="btn btn-primary"
                    disabled={
                      saving
                    }
                  >
                    {saving ? (
                      <>
                        <span className="spinner-border spinner-border-sm me-2" />
                        Đang lưu...
                      </>
                    ) : editingItem ? (
                      'Lưu thay đổi'
                    ) : (
                      'Thêm mới'
                    )}
                  </button>

                </div>

              </form>
            </div>
          </div>
        </div>
      )}

    </div>
  );
}