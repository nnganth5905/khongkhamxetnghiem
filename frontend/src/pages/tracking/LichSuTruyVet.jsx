// src/pages/tracking/LichSuTruyVet.jsx

import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import {
  getTrackingHistory,
  getTrackingStatusLabel,
} from '../../services/trackingService';

import {
  getApiErrorMessage,
} from '../../services/api';

// ============================================================
// FORMAT
// ============================================================

const formatDateTime = (value) => {
  if (!value) {
    return '—';
  }

  const date =
    new Date(value);

  if (!Number.isNaN(date.getTime())) {
    return date.toLocaleString(
      'vi-VN',
      {
        hour: '2-digit',
        minute: '2-digit',
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
      }
    );
  }

  return String(value);
};

const objectTypeLabel = (type) => {
  switch (
    String(type || '').toLowerCase()
  ) {
    case 'datlichkham':
      return 'Lịch khám';

    case 'datlichxetnghiem':
      return 'Lịch xét nghiệm';

    case 'luotkham':
      return 'Lượt khám';

    case 'luotxetnghiem':
      return 'Lượt xét nghiệm';

    case 'phieuxetnghiem':
      return 'Phiếu xét nghiệm';

    case 'maubenhpham':
      return 'Mẫu bệnh phẩm';

    case 'worklist':
      return 'Worklist';

    case 'ketquaxetnghiem':
      return 'Kết quả xét nghiệm';

    default:
      return type || 'Khác';
  }
};

const objectTypeClass = (type) => {
  switch (
    String(type || '').toLowerCase()
  ) {
    case 'datlichkham':
    case 'luotkham':
      return 'bg-primary-subtle text-primary';

    case 'datlichxetnghiem':
    case 'luotxetnghiem':
      return 'bg-success-subtle text-success';

    case 'maubenhpham':
      return 'bg-info-subtle text-info-emphasis';

    case 'worklist':
      return 'bg-warning-subtle text-warning-emphasis';

    case 'ketquaxetnghiem':
      return 'bg-danger-subtle text-danger';

    default:
      return 'bg-secondary-subtle text-secondary';
  }
};

// ============================================================
// COMPONENT
// ============================================================

export default function LichSuTruyVet() {
  const [items, setItems] =
    useState([]);

  const [keyword, setKeyword] =
    useState('');

  const [type, setType] =
    useState('');

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState('');

  // ==========================================================
  // LOAD
  // ==========================================================

  const loadHistory = async () => {
    try {
      setLoading(true);
      setError('');

      const data =
        await getTrackingHistory({
          type:
            type || undefined,

          limit: 300,
        });

      setItems(
        Array.isArray(data)
          ? data
          : []
      );
    } catch (err) {
      setItems([]);

      setError(
        getApiErrorMessage(
          err,
          'Không thể tải lịch sử truy vết.'
        )
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadHistory();
  }, [type]);

  // ==========================================================
  // FILTER
  // ==========================================================

  const filteredItems =
    useMemo(() => {
      const q =
        keyword
          .trim()
          .toLowerCase();

      if (!q) {
        return items;
      }

      return items.filter(
        (item) =>
          [
            item.patientCode,
            item.objectType,
            item.objectId,
            item.action,
            item.description,
            item.oldStatus,
            item.newStatus,
          ]
            .filter(Boolean)
            .join(' ')
            .toLowerCase()
            .includes(q)
      );
    }, [
      items,
      keyword,
    ]);

  return (
    <div className="container-fluid py-4">
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Lịch sử truy vết
          </h1>

          <p className="text-secondary mb-0">
            Nhật ký thay đổi trạng thái trong quá trình
            khám và xét nghiệm.
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-primary"
          disabled={loading}
          onClick={loadHistory}
        >
          <i className="fa-solid fa-rotate me-2" />
          Làm mới
        </button>
      </div>

      {error && (
        <Notification
          type="danger"
          message={error}
          onClose={() =>
            setError('')
          }
        />
      )}

      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body p-4">
          {/* FILTER */}

          <div className="row g-3 mb-4">
            <div className="col-lg-8">
              <div className="input-group">
                <span className="input-group-text bg-white">
                  <i className="fa-solid fa-magnifying-glass text-secondary" />
                </span>

                <input
                  type="search"
                  className="form-control"
                  placeholder="Tìm hành động, mã đối tượng, trạng thái..."
                  value={keyword}
                  onChange={(e) =>
                    setKeyword(
                      e.target.value
                    )
                  }
                />
              </div>
            </div>

            <div className="col-lg-4">
              <select
                className="form-select"
                value={type}
                onChange={(e) =>
                  setType(
                    e.target.value
                  )
                }
              >
                <option value="">
                  Tất cả loại dữ liệu
                </option>

                <option value="datlichkham">
                  Lịch khám
                </option>

                <option value="luotkham">
                  Lượt khám
                </option>

                <option value="datlichxetnghiem">
                  Lịch xét nghiệm
                </option>

                <option value="luotxetnghiem">
                  Lượt xét nghiệm
                </option>

                <option value="phieuxetnghiem">
                  Phiếu xét nghiệm
                </option>

                <option value="maubenhpham">
                  Mẫu bệnh phẩm
                </option>

                <option value="worklist">
                  Worklist
                </option>

                <option value="ketquaxetnghiem">
                  Kết quả xét nghiệm
                </option>
              </select>
            </div>
          </div>

          {loading ? (
            <Loading text="Đang tải lịch sử truy vết..." />
          ) : (
            <>
              <div className="d-flex justify-content-between align-items-center mb-3">
                <small className="text-secondary">
                  Tổng số:{' '}
                  <strong>
                    {filteredItems.length}
                  </strong>{' '}
                  bản ghi
                </small>
              </div>

              <div className="table-responsive">
                <table className="table table-hover align-middle">
                  <thead className="table-light">
                    <tr>
                      <th style={{
                        width: 70,
                      }}>
                        #
                      </th>

                      <th>
                        Thời gian
                      </th>

                      <th>
                        Loại
                      </th>

                      <th>
                        Mã
                      </th>

                      <th>
                        Hành động
                      </th>

                      <th>
                        Thay đổi trạng thái
                      </th>

                      <th>
                        Nguồn
                      </th>
                    </tr>
                  </thead>

                  <tbody>
                    {filteredItems.length >
                    0 ? (
                      filteredItems.map(
                        (
                          item,
                          index
                        ) => (
                          <tr
                            key={
                              item.id ||
                              index
                            }
                          >
                            <td className="text-secondary">
                              {index +
                                1}
                            </td>

                            <td className="text-nowrap">
                              {formatDateTime(
                                item.time
                              )}
                            </td>

                            <td>
                              <span
                                className={`badge ${objectTypeClass(
                                  item.objectType
                                )}`}
                              >
                                {objectTypeLabel(
                                  item.objectType
                                )}
                              </span>
                            </td>

                            <td>
                              <div className="fw-semibold">
                                {item.objectId ||
                                  '—'}
                              </div>

                              {item.patientCode && (
                                <small className="text-secondary">
                                  KH:{' '}
                                  {
                                    item.patientCode
                                  }
                                </small>
                              )}
                            </td>

                            <td>
                              <div className="fw-semibold">
                                {item.action ||
                                  'Cập nhật'}
                              </div>

                              {item.description && (
                                <small className="text-secondary">
                                  {
                                    item.description
                                  }
                                </small>
                              )}
                            </td>

                            <td>
                              {item.oldStatus ||
                              item.newStatus ? (
                                <div className="d-flex flex-wrap align-items-center gap-2">
                                  {item.oldStatus && (
                                    <span className="badge bg-secondary-subtle text-secondary">
                                      {getTrackingStatusLabel(
                                        item.oldStatus
                                      )}
                                    </span>
                                  )}

                                  {item.oldStatus &&
                                    item.newStatus && (
                                      <i className="fa-solid fa-arrow-right small text-secondary" />
                                    )}

                                  {item.newStatus && (
                                    <span className="badge bg-primary-subtle text-primary">
                                      {getTrackingStatusLabel(
                                        item.newStatus
                                      )}
                                    </span>
                                  )}
                                </div>
                              ) : (
                                <span className="text-secondary">
                                  —
                                </span>
                              )}
                            </td>

                            <td>
                              <span
                                className={`badge ${
                                  item.source ===
                                  'system'
                                    ? 'bg-dark-subtle text-dark'
                                    : 'bg-info-subtle text-info-emphasis'
                                }`}
                              >
                                {item.source ===
                                'system'
                                  ? 'Hệ thống'
                                  : 'Người dùng'}
                              </span>
                            </td>
                          </tr>
                        )
                      )
                    ) : (
                      <tr>
                        <td
                          colSpan="7"
                          className="text-center py-5 text-secondary"
                        >
                          <i className="fa-solid fa-clock-rotate-left fs-2 d-block mb-3 opacity-50" />

                          Chưa có lịch sử truy vết phù hợp.
                        </td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
}