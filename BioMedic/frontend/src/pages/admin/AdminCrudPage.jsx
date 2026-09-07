import React, { useEffect, useMemo, useState } from 'react';

import Loading from '../Loading';
import Notification from '../Notification';
import { getApiErrorMessage } from '../../services/api';

const EMPTY_MESSAGE = {
  type: '',
  text: '',
};

const getByPath = (obj, key) => {
  if (!obj || !key) return undefined;

  return key
    .split('.')
    .reduce(
      (value, part) =>
        value == null
          ? undefined
          : value[part],
      obj
    );
};

const renderCell = (row, column) => {
  const rawValue =
    typeof column.value === 'function'
      ? column.value(row)
      : getByPath(
          row,
          column.key
        );

  if (
    rawValue === null ||
    rawValue === undefined ||
    rawValue === ''
  ) {
    return '—';
  }

  if (column.type === 'status') {
    const value =
      String(rawValue);

    const upper =
      value.toUpperCase();

    const active =
      [
        'ACTIVE',
        'ENABLED',
        'WORKING',
        'AVAILABLE',
        'YES',
        '1',
        'TRUE',
        'HOAT_DONG',
        'ĐANG HOẠT ĐỘNG',
      ].includes(upper);

    return (
      <span
        className={`badge ${
          active
            ? 'bg-success-subtle text-success'
            : 'bg-secondary-subtle text-secondary'
        }`}
      >
        {column.format
          ? column.format(rawValue)
          : value}
      </span>
    );
  }

  if (column.type === 'money') {
    const number =
      Number(rawValue);

    return Number.isFinite(
      number
    )
      ? `${number.toLocaleString(
          'vi-VN'
        )} đ`
      : rawValue;
  }

  if (column.format) {
    return column.format(
      rawValue,
      row
    );
  }

  return String(rawValue);
};

const buildInitialForm = (
  fields,
  item = null
) => {
  return fields.reduce(
    (acc, field) => {
      let value =
        item
          ? getByPath(
              item,
              field.key
            )
          : field.defaultValue ??
            '';

      if (
        value === null ||
        value === undefined
      ) {
        value = '';
      }

      if (
        field.type ===
          'checkbox'
      ) {
        value = Boolean(value);
      }

      acc[field.key] = value;
      return acc;
    },
    {}
  );
};

export default function AdminCrudPage({
  title,
  subtitle,
  icon = 'fa-solid fa-table-list',
  columns = [],
  fields = [],
  loadItems,
  createItem,
  updateItem,
  deleteItem,
  itemId = (row) =>
    row.id,
  searchPlaceholder =
    'Tìm kiếm...',
  addButtonText =
    'Thêm mới',
  formTitleCreate =
    'Thêm mới',
  formTitleEdit =
    'Chỉnh sửa',
  emptyText =
    'Chưa có dữ liệu.',
  canDelete = true,
  canEdit = true,
  extraToolbar = null,
}) {
  const [items, setItems] =
    useState([]);

  const [keyword, setKeyword] =
    useState('');

  const [loading, setLoading] =
    useState(true);

  const [saving, setSaving] =
    useState(false);

  const [
    deletingId,
    setDeletingId,
  ] = useState(null);

  const [
    editingItem,
    setEditingItem,
  ] = useState(null);

  const [showForm, setShowForm] =
    useState(false);

  const [form, setForm] =
    useState(
      buildInitialForm(
        fields
      )
    );

  const [message, setMessage] =
    useState(EMPTY_MESSAGE);

  const load = async () => {
    try {
      setLoading(true);

      const data =
        await loadItems();

      const list =
        Array.isArray(data)
          ? data
          : data?.content ||
            data?.items ||
            data?.data ||
            [];

      setItems(list);
    } catch (error) {
      setMessage({
        type: 'danger',

        text: getApiErrorMessage(
          error,
          `Không thể tải ${title.toLowerCase()}.`
        ),
      });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const searchableRows =
    useMemo(() => {
      const q =
        keyword
          .trim()
          .toLowerCase();

      if (!q) return items;

      return items.filter(
        (row) =>
          columns.some(
            (column) => {
              const value =
                typeof column.value ===
                'function'
                  ? column.value(
                      row
                    )
                  : getByPath(
                      row,
                      column.key
                    );

              return String(
                value ?? ''
              )
                .toLowerCase()
                .includes(q);
            }
          )
      );
    }, [
      items,
      keyword,
      columns,
    ]);

  const openCreate = () => {
    setEditingItem(null);

    setForm(
      buildInitialForm(
        fields
      )
    );

    setShowForm(true);
  };

  const openEdit = (row) => {
    setEditingItem(row);

    setForm(
      buildInitialForm(
        fields,
        row
      )
    );

    setShowForm(true);
  };

  const closeForm = () => {
    if (saving) return;

    setShowForm(false);
    setEditingItem(null);

    setForm(
      buildInitialForm(
        fields
      )
    );
  };

  const changeField = (
    field,
    value
  ) => {
    setForm((prev) => ({
      ...prev,
      [field.key]: value,
    }));
  };

  const handleSubmit =
    async (e) => {
      e.preventDefault();

      try {
        setSaving(true);

        const payload =
          fields.reduce(
            (acc, field) => {
              let value =
                form[field.key];

              if (
                field.transform
              ) {
                value =
                  field.transform(
                    value,
                    form
                  );
              }

              acc[field.key] =
                value;

              return acc;
            },
            {}
          );

        if (editingItem) {
          await updateItem(
            itemId(
              editingItem
            ),
            payload
          );

          setMessage({
            type: 'success',
            text:
              'Cập nhật thành công.',
          });
        } else {
          await createItem(
            payload
          );

          setMessage({
            type: 'success',
            text:
              'Thêm mới thành công.',
          });
        }

        closeForm();
        await load();
      } catch (error) {
        setMessage({
          type: 'danger',

          text: getApiErrorMessage(
            error,
            editingItem
              ? 'Không thể cập nhật dữ liệu.'
              : 'Không thể thêm mới dữ liệu.'
          ),
        });
      } finally {
        setSaving(false);
      }
    };

  const handleDelete =
    async (row) => {
      const id =
        itemId(row);

      if (
        id === null ||
        id === undefined
      ) {
        setMessage({
          type: 'danger',

          text:
            'Không xác định được ID bản ghi.',
        });

        return;
      }

      const name =
        columns.length > 0
          ? renderCell(
              row,
              columns[0]
            )
          : id;

      const confirmed =
        window.confirm(
          `Bạn có chắc chắn muốn xóa "${String(
            name
          )}"?`
        );

      if (!confirmed) return;

      try {
        setDeletingId(id);

        await deleteItem(id);

        setMessage({
          type: 'success',

          text:
            'Xóa dữ liệu thành công.',
        });

        await load();
      } catch (error) {
        setMessage({
          type: 'danger',

          text: getApiErrorMessage(
            error,
            'Không thể xóa dữ liệu. Bản ghi có thể đang được sử dụng bởi dữ liệu khác.'
          ),
        });
      } finally {
        setDeletingId(null);
      }
    };

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            {title}
          </h1>

          {subtitle && (
            <p className="text-secondary mb-0">
              {subtitle}
            </p>
          )}
        </div>

        <div className="d-flex flex-wrap gap-2">
          {extraToolbar}

          <button
            type="button"
            className="btn btn-outline-primary"
            onClick={load}
            disabled={loading}
          >
            <i className="fa-solid fa-rotate me-2" />
            Làm mới
          </button>

          <button
            type="button"
            className="btn btn-primary"
            onClick={openCreate}
          >
            <i className="fa-solid fa-plus me-2" />
            {addButtonText}
          </button>
        </div>
      </div>

      {message.text && (
        <Notification
          type={message.type}
          message={message.text}
          onClose={() =>
            setMessage(
              EMPTY_MESSAGE
            )
          }
        />
      )}

      {showForm && (
        <div className="card border-0 shadow-sm rounded-4 mb-4">
          <div className="card-body p-4">
            <div className="d-flex justify-content-between align-items-center gap-3 mb-4">
              <div className="d-flex align-items-center gap-3">
                <div
                  className="d-flex align-items-center justify-content-center rounded-circle"
                  style={{
                    width: 46,
                    height: 46,
                    background:
                      '#eaf2ff',
                    color:
                      'var(--primary)',
                  }}
                >
                  <i
                    className={icon}
                  />
                </div>

                <h5 className="fw-bold mb-0">
                  {editingItem
                    ? formTitleEdit
                    : formTitleCreate}
                </h5>
              </div>

              <button
                type="button"
                className="btn btn-sm btn-light"
                onClick={closeForm}
              >
                <i className="fa-solid fa-xmark" />
              </button>
            </div>

            <form
              onSubmit={
                handleSubmit
              }
            >
              <div className="row g-3">
                {fields.map(
                  (field) => (
                    <div
                      className={
                        field.colClass ??
                        'col-md-6'
                      }
                      key={
                        field.key
                      }
                    >
                      {field.type ===
                      'checkbox' ? (
                        <div className="form-check mt-4">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            id={`field-${field.key}`}
                            checked={Boolean(
                              form[
                                field
                                  .key
                              ]
                            )}
                            onChange={(
                              e
                            ) =>
                              changeField(
                                field,
                                e
                                  .target
                                  .checked
                              )
                            }
                          />

                          <label
                            className="form-check-label"
                            htmlFor={`field-${field.key}`}
                          >
                            {
                              field.label
                            }
                          </label>
                        </div>
                      ) : (
                        <>
                          <label className="form-label fw-semibold">
                            {
                              field.label
                            }

                            {field.required && (
                              <span className="text-danger">
                                {' '}
                                *
                              </span>
                            )}
                          </label>

                          {field.type ===
                          'select' ? (
                            <select
                              className="form-select"
                              value={
                                form[
                                  field
                                    .key
                                ] ??
                                ''
                              }
                              required={
                                field.required
                              }
                              disabled={
                                field.disabled &&
                                editingItem
                              }
                              onChange={(
                                e
                              ) =>
                                changeField(
                                  field,
                                  e
                                    .target
                                    .value
                                )
                              }
                            >
                              <option value="">
                                -- Chọn --
                              </option>

                              {(
                                field.options ??
                                []
                              ).map(
                                (
                                  option
                                ) => (
                                  <option
                                    key={
                                      option.value
                                    }
                                    value={
                                      option.value
                                    }
                                  >
                                    {
                                      option.label
                                    }
                                  </option>
                                )
                              )}
                            </select>
                          ) : field.type ===
                            'textarea' ? (
                            <textarea
                              className="form-control"
                              rows={
                                field.rows ??
                                3
                              }
                              placeholder={
                                field.placeholder ??
                                ''
                              }
                              value={
                                form[
                                  field
                                    .key
                                ] ??
                                ''
                              }
                              required={
                                field.required
                              }
                              onChange={(
                                e
                              ) =>
                                changeField(
                                  field,
                                  e
                                    .target
                                    .value
                                )
                              }
                            />
                          ) : (
                            <input
                              type={
                                field.type ??
                                'text'
                              }
                              className="form-control"
                              placeholder={
                                field.placeholder ??
                                ''
                              }
                              value={
                                form[
                                  field
                                    .key
                                ] ??
                                ''
                              }
                              required={
                                field.required
                              }
                              min={
                                field.min
                              }
                              max={
                                field.max
                              }
                              step={
                                field.step
                              }
                              disabled={
                                field.disabled &&
                                editingItem
                              }
                              onChange={(
                                e
                              ) =>
                                changeField(
                                  field,
                                  e
                                    .target
                                    .value
                                )
                              }
                            />
                          )}

                          {field.help && (
                            <div className="form-text">
                              {
                                field.help
                              }
                            </div>
                          )}
                        </>
                      )}
                    </div>
                  )
                )}
              </div>

              <div className="d-flex flex-wrap gap-2 mt-4">
                <button
                  type="submit"
                  className="btn btn-primary"
                  disabled={saving}
                >
                  <i className="fa-regular fa-floppy-disk me-2" />

                  {saving
                    ? 'Đang lưu...'
                    : editingItem
                    ? 'Lưu thay đổi'
                    : 'Thêm mới'}
                </button>

                <button
                  type="button"
                  className="btn btn-outline-secondary"
                  disabled={saving}
                  onClick={closeForm}
                >
                  Hủy
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body p-4">
          <div className="input-group mb-4">
            <span className="input-group-text bg-white">
              <i className="fa-solid fa-magnifying-glass text-secondary" />
            </span>

            <input
              type="search"
              className="form-control"
              placeholder={
                searchPlaceholder
              }
              value={keyword}
              onChange={(e) =>
                setKeyword(
                  e.target.value
                )
              }
            />
          </div>

          {loading ? (
            <Loading
              text={`Đang tải ${title.toLowerCase()}...`}
            />
          ) : (
            <div className="table-responsive">
              <table className="table table-hover align-middle">
                <thead className="table-light">
                  <tr>
                    <th
                      style={{
                        width: 60,
                      }}
                    >
                      #
                    </th>

                    {columns.map(
                      (column) => (
                        <th
                          key={
                            column.key ??
                            column.label
                          }
                          className={
                            column.className
                          }
                        >
                          {
                            column.label
                          }
                        </th>
                      )
                    )}

                    {(canEdit ||
                      canDelete) && (
                      <th className="text-end">
                        Thao tác
                      </th>
                    )}
                  </tr>
                </thead>

                <tbody>
                  {searchableRows.length >
                  0 ? (
                    searchableRows.map(
                      (
                        row,
                        index
                      ) => (
                        <tr
                          key={
                            itemId(
                              row
                            ) ??
                            index
                          }
                        >
                          <td>
                            {index +
                              1}
                          </td>

                          {columns.map(
                            (
                              column
                            ) => (
                              <td
                                key={
                                  column.key ??
                                  column.label
                                }
                                className={
                                  column
                                    .cellClassName
                                }
                              >
                                {renderCell(
                                  row,
                                  column
                                )}
                              </td>
                            )
                          )}

                          {(canEdit ||
                            canDelete) && (
                            <td className="text-end text-nowrap">
                              {canEdit && (
                                <button
                                  type="button"
                                  className="btn btn-sm btn-outline-primary me-2"
                                  onClick={() =>
                                    openEdit(
                                      row
                                    )
                                  }
                                >
                                  <i className="fa-solid fa-pen me-1" />
                                  Sửa
                                </button>
                              )}

                              {canDelete && (
                                <button
                                  type="button"
                                  className="btn btn-sm btn-outline-danger"
                                  disabled={
                                    deletingId ===
                                    itemId(
                                      row
                                    )
                                  }
                                  onClick={() =>
                                    handleDelete(
                                      row
                                    )
                                  }
                                >
                                  <i className="fa-regular fa-trash-can me-1" />
                                  Xóa
                                </button>
                              )}
                            </td>
                          )}
                        </tr>
                      )
                    )
                  ) : (
                    <tr>
                      <td
                        colSpan={
                          columns.length +
                          (canEdit ||
                          canDelete
                            ? 2
                            : 1)
                        }
                        className="text-center text-secondary py-5"
                      >
                        {emptyText}
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}