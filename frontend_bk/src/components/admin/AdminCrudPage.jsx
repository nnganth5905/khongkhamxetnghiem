import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

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

  return [];
}

function getColumnKey(column, index) {
  if (typeof column === 'string') {
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

function getColumnLabel(column) {
  if (typeof column === 'string') {
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

function resolveValue(row, column, index) {
  if (
    column &&
    typeof column.render === 'function'
  ) {
    return column.render(row);
  }

  const key =
    getColumnKey(column, index);

  return row?.[key];
}

function makeInitialForm(
  fields,
  initialValues,
) {
  const form = {
    ...(initialValues || {}),
  };

  for (const field of fields || []) {
    const key =
      typeof field === 'string'
        ? field
        : (
            field?.name ||
            field?.key ||
            field?.field
          );

    if (
      key &&
      form[key] === undefined
    ) {
      form[key] =
        field?.defaultValue ?? '';
    }
  }

  return form;
}

export default function AdminCrudPage(props) {
  const {
    title = 'Quản lý dữ liệu',
    description = '',
    columns = [],
    fields = [],
    idKey = 'id',
    initialValues = {},
    searchPlaceholder = 'Tìm kiếm...',
  } = props;

  const service =
    props.service || {};

  const loadFn =
    props.loadData ||
    props.fetchData ||
    props.getData ||
    props.listData ||
    props.onLoad ||
    service.list ||
    service.getAll;

  const createFn =
    props.createData ||
    props.createItem ||
    props.onCreate ||
    service.create;

  const updateFn =
    props.updateData ||
    props.updateItem ||
    props.onUpdate ||
    service.update;

  const deleteFn =
    props.deleteData ||
    props.deleteItem ||
    props.onDelete ||
    service.remove ||
    service.delete;

  const [items, setItems] =
    useState([]);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState('');

  const [message, setMessage] =
    useState('');

  const [query, setQuery] =
    useState('');

  const [showForm, setShowForm] =
    useState(false);

  const [editingItem, setEditingItem] =
    useState(null);

  const [form, setForm] =
    useState(
      makeInitialForm(
        fields,
        initialValues,
      ),
    );

  async function reload() {
    setLoading(true);
    setError('');

    try {
      if (
        typeof loadFn !== 'function'
      ) {
        setItems([]);
        return;
      }

      const payload =
        await loadFn();

      setItems(
        normalizeList(payload),
      );
    } catch (err) {
      setError(
        err?.response?.data?.message ||
        err?.message ||
        'Không thể tải dữ liệu.',
      );
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    reload();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

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
        (item) =>
          Object.values(item || {})
            .some((value) =>
              String(value ?? '')
                .toLowerCase()
                .includes(keyword),
            ),
      );
    }, [items, query]);

  function openCreate() {
    setEditingItem(null);

    setForm(
      makeInitialForm(
        fields,
        initialValues,
      ),
    );

    setShowForm(true);
    setError('');
    setMessage('');
  }

  function openEdit(item) {
    setEditingItem(item);

    setForm({
      ...makeInitialForm(
        fields,
        initialValues,
      ),
      ...item,
    });

    setShowForm(true);
    setError('');
    setMessage('');
  }

  function closeForm() {
    setShowForm(false);
    setEditingItem(null);

    setForm(
      makeInitialForm(
        fields,
        initialValues,
      ),
    );
  }

  function updateForm(
    key,
    value,
  ) {
    setForm((current) => ({
      ...current,
      [key]: value,
    }));
  }

  async function handleSubmit(event) {
    event.preventDefault();

    setError('');
    setMessage('');

    try {
      if (editingItem) {
        if (
          typeof updateFn !== 'function'
        ) {
          throw new Error(
            'Trang chưa được cấu hình hàm cập nhật.',
          );
        }

        const id =
          editingItem?.[idKey] ??
          editingItem?.id;

        /*
         * Hỗ trợ cả:
         * update(id, payload)
         * và onUpdate(payload, item)
         */
        if (
          props.onUpdate &&
          !props.updateData &&
          !props.updateItem
        ) {
          await updateFn(
            form,
            editingItem,
          );
        } else {
          await updateFn(
            id,
            form,
          );
        }

        setMessage(
          'Cập nhật thành công.',
        );
      } else {
        if (
          typeof createFn !== 'function'
        ) {
          throw new Error(
            'Trang chưa được cấu hình hàm thêm mới.',
          );
        }

        await createFn(form);

        setMessage(
          'Thêm mới thành công.',
        );
      }

      closeForm();
      await reload();
    } catch (err) {
      setError(
        err?.response?.data?.message ||
        err?.message ||
        'Không thể lưu dữ liệu.',
      );
    }
  }

  async function handleDelete(item) {
    if (
      typeof deleteFn !== 'function'
    ) {
      setError(
        'Trang chưa được cấu hình hàm xóa.',
      );
      return;
    }

    const ok =
      window.confirm(
        'Bạn có chắc muốn xóa dữ liệu này?',
      );

    if (!ok) {
      return;
    }

    setError('');
    setMessage('');

    try {
      const id =
        item?.[idKey] ??
        item?.id;

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
        await deleteFn(id);
      }

      setMessage(
        'Xóa thành công.',
      );

      await reload();
    } catch (err) {
      setError(
        err?.response?.data?.message ||
        err?.message ||
        'Không thể xóa dữ liệu.',
      );
    }
  }

  const actualColumns =
    columns.length > 0
      ? columns
      : (
          items[0]
            ? Object
                .keys(items[0])
                .slice(0, 6)
            : []
        );

  return (
    <div className="container-fluid py-4">
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
          onClick={openCreate}
          disabled={
            typeof createFn !==
            'function'
          }
        >
          <i className="fa-solid fa-plus me-2" />
          Thêm mới
        </button>
      </div>

      {message && (
        <div className="alert alert-success">
          {message}
        </div>
      )}

      {error && (
        <div className="alert alert-danger">
          {error}
        </div>
      )}

      <div className="card border-0 shadow-sm">
        <div className="card-body">
          <div className="mb-3">
            <input
              type="search"
              className="form-control"
              placeholder={
                searchPlaceholder
              }
              value={query}
              onChange={(event) =>
                setQuery(
                  event.target.value,
                )
              }
            />
          </div>

          {loading ? (
            <div className="text-center py-5">
              <div
                className="spinner-border"
                style={{
                  color:
                    'var(--primary, #0360D9)',
                }}
              />
            </div>
          ) : (
            <div className="table-responsive">
              <table className="table table-hover align-middle">
                <thead>
                  <tr>
                    {actualColumns.map(
                      (column, index) => (
                        <th
                          key={getColumnKey(
                            column,
                            index,
                          )}
                        >
                          {getColumnLabel(
                            column,
                          )}
                        </th>
                      ),
                    )}

                    <th
                      className="text-end"
                      style={{
                        minWidth: 145,
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
                      (item, rowIndex) => (
                        <tr
                          key={String(
                            item?.[idKey] ??
                              item?.id ??
                              rowIndex,
                          )}
                        >
                          {actualColumns.map(
                            (
                              column,
                              columnIndex,
                            ) => (
                              <td
                                key={getColumnKey(
                                  column,
                                  columnIndex,
                                )}
                              >
                                {String(
                                  resolveValue(
                                    item,
                                    column,
                                    columnIndex,
                                  ) ?? '',
                                )}
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
                              }
                            >
                              Xóa
                            </button>
                          </td>
                        </tr>
                      ),
                    )
                  )}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>

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
                <div className="modal-header">
                  <h5 className="modal-title">
                    {editingItem
                      ? 'Cập nhật'
                      : 'Thêm mới'}
                  </h5>

                  <button
                    type="button"
                    className="btn-close"
                    onClick={closeForm}
                  />
                </div>

                <div className="modal-body">
                  {fields.length === 0 ? (
                    <div className="alert alert-info mb-0">
                      Trang chưa khai báo
                      danh sách trường nhập
                      liệu.
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
                                  name: field,
                                  label:
                                    field,
                                }
                              : field;

                          const name =
                            config.name ||
                            config.key ||
                            config.field;

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

                          return (
                            <div
                              className={
                                config.className ||
                                'col-12 col-md-6'
                              }
                              key={
                                name ||
                                index
                              }
                            >
                              <label className="form-label">
                                {label}
                                {config.required &&
                                  ' *'}
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
                                  onChange={(
                                    event,
                                  ) =>
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
                                          key={String(
                                            value ??
                                              optionIndex,
                                          )}
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
                                  onChange={(
                                    event,
                                  ) =>
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
                                  type={type}
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
                                  onChange={(
                                    event,
                                  ) =>
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

                <div className="modal-footer">
                  <button
                    type="button"
                    className="btn btn-light"
                    onClick={
                      closeForm
                    }
                  >
                    Hủy
                  </button>

                  <button
                    type="submit"
                    className="btn btn-primary"
                  >
                    {editingItem
                      ? 'Lưu thay đổi'
                      : 'Thêm mới'}
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
