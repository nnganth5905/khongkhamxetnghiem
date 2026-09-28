// src/pages/dashboard/TechnicianDashboard.jsx

import React, {
  useEffect,
  useState,
} from 'react';

import {
  Link,
} from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import {
  getTechnicianDashboard,
} from '../../services/dashboardService';

import {
  getApiErrorMessage,
} from '../../services/api';

// ============================================================
// FALLBACK
// ============================================================

const FALLBACK = {
  technicianId: '',

  waitingSpecimens: 0,

  receivedSpecimens: 0,

  inProgress: 0,

  pendingResultEntries: 0,

  worklist: [],
};

// ============================================================
// STATUS
// ============================================================

const workStatusText = (status) => {
  switch (
    String(status || '').toLowerCase()
  ) {
    case 'queue':
      return 'Chờ thực hiện';

    case 'running':
      return 'Đang xét nghiệm';

    case 'to_result':
      return 'Chờ nhập kết quả';

    case 'rerun':
      return 'Cần chạy lại';

    case 'finished':
      return 'Hoàn tất';

    case 'cancelled':
      return 'Đã hủy';

    default:
      return status || '—';
  }
};

const workStatusClass = (status) => {
  switch (
    String(status || '').toLowerCase()
  ) {
    case 'queue':
      return 'bg-secondary-subtle text-secondary';

    case 'running':
      return 'bg-warning-subtle text-warning-emphasis';

    case 'to_result':
      return 'bg-primary-subtle text-primary';

    case 'rerun':
      return 'bg-danger-subtle text-danger';

    case 'finished':
      return 'bg-success-subtle text-success';

    default:
      return 'bg-secondary-subtle text-secondary';
  }
};

// ============================================================
// DATE
// ============================================================

const formatDateTime = (value) => {
  if (!value) {
    return '—';
  }

  const date =
    new Date(value);

  if (
    Number.isNaN(
      date.getTime()
    )
  ) {
    return String(value);
  }

  return date.toLocaleString(
    'vi-VN',
    {
      hour: '2-digit',
      minute: '2-digit',
      day: '2-digit',
      month: '2-digit',
    }
  );
};

// ============================================================
// COMPONENT
// ============================================================

export default function TechnicianDashboard() {
  const [
    data,
    setData,
  ] = useState(FALLBACK);

  const [
    loading,
    setLoading,
  ] = useState(true);

  const [
    refreshing,
    setRefreshing,
  ] = useState(false);

  const [
    error,
    setError,
  ] = useState('');

  // ==========================================================
  // LOAD
  // ==========================================================

  const load = async (
    full = true
  ) => {
    try {
      if (full) {
        setLoading(true);
      } else {
        setRefreshing(true);
      }

      setError('');

      const response =
        await getTechnicianDashboard();

      setData({
        ...FALLBACK,
        ...(response || {}),
      });
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          'Không thể tải Dashboard kỹ thuật viên.'
        )
      );
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  if (loading) {
    return (
      <Loading text="Đang tải Dashboard kỹ thuật viên..." />
    );
  }

  const cards = [
    {
      label:
        'Mẫu chờ tiếp nhận',

      value:
        data.waitingSpecimens ??
        0,

      icon:
        'fa-solid fa-vial',

      to:
        '/technician/mau-benh-pham',

      bg:
        '#fff8e6',

      color:
        '#b7791f',
    },

    {
      label:
        'Mẫu đã tiếp nhận',

      value:
        data.receivedSpecimens ??
        0,

      icon:
        'fa-solid fa-box-open',

      to:
        '/technician/worklist',

      bg:
        '#eef4ff',

      color:
        '#0d6efd',
    },

    {
      label:
        'Đang xét nghiệm',

      value:
        data.inProgress ??
        0,

      icon:
        'fa-solid fa-flask-vial',

      to:
        '/technician/worklist',

      bg:
        '#eefaf2',

      color:
        '#198754',
    },

    {
      label:
        'Chờ nhập kết quả',

      value:
        data.pendingResultEntries ??
        0,

      icon:
        'fa-solid fa-keyboard',

      to:
        '/technician/worklist',

      bg:
        '#f2efff',

      color:
        '#6f42c1',
    },
  ];

  const worklist =
    Array.isArray(
      data.worklist
    )
      ? data.worklist
      : [];

  return (
    <div>
      {/* HEADER */}

      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Dashboard kỹ thuật viên
          </h1>

          <p className="text-secondary mb-0">
            KTV hiện tại:{' '}

            <strong>
              {data.technicianId ||
                '—'}
            </strong>
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-primary"
          disabled={refreshing}
          onClick={() =>
            load(false)
          }
        >
          <i
            className={`fa-solid fa-rotate me-2 ${
              refreshing
                ? 'fa-spin'
                : ''
            }`}
          />

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

      {/* CARDS */}

      <div className="row g-4 mb-4">
        {cards.map(
          (card) => (
            <div
              className="col-md-6 col-xl-3"
              key={card.label}
            >
              <Link
                to={card.to}
                className="card border-0 shadow-sm rounded-4 h-100 text-decoration-none text-dark"
              >
                <div className="card-body p-4">
                  <div className="d-flex justify-content-between">
                    <div>
                      <div className="small text-secondary">
                        {card.label}
                      </div>

                      <div className="fs-2 fw-bold mt-2">
                        {card.value}
                      </div>
                    </div>

                    <div
                      className="rounded-3 d-flex justify-content-center align-items-center"
                      style={{
                        width: 50,
                        height: 50,
                        background:
                          card.bg,
                        color:
                          card.color,
                      }}
                    >
                      <i
                        className={`${card.icon} fs-5`}
                      />
                    </div>
                  </div>
                </div>
              </Link>
            </div>
          )
        )}
      </div>

      {/* QUICK LINKS */}

      <div className="row g-4 mb-4">
        <div className="col-md-6">
          <Link
            to="/technician/mau-benh-pham"
            className="btn btn-outline-primary w-100 py-3 rounded-4"
          >
            <i className="fa-solid fa-vials me-2" />
            Danh sách mẫu
          </Link>
        </div>

        <div className="col-md-6">
          <Link
            to="/technician/worklist"
            className="btn btn-primary w-100 py-3 rounded-4"
          >
            <i className="fa-solid fa-list-check me-2" />
            Worklist
          </Link>
        </div>
      </div>

      {/* WORKLIST */}

      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body p-4">
          <div className="d-flex justify-content-between align-items-center mb-4">
            <div>
              <h5 className="fw-bold mb-1">
                Công việc cần xử lý
              </h5>

              <small className="text-secondary">
                Worklist được phân công cho kỹ thuật viên hiện tại.
              </small>
            </div>

            <Link
              to="/technician/worklist"
              className="small text-decoration-none"
            >
              Xem tất cả
            </Link>
          </div>

          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead className="table-light">
                <tr>
                  <th>
                    Mã mẫu
                  </th>

                  <th>
                    Người bệnh
                  </th>

                  <th>
                    Xét nghiệm
                  </th>

                  <th>
                    Tiếp nhận
                  </th>

                  <th>
                    Trạng thái
                  </th>

                  <th />
                </tr>
              </thead>

              <tbody>
                {worklist.length >
                0 ? (
                  worklist.map(
                    (
                      item,
                      index
                    ) => (
                      <tr
                        key={
                          item.id ??
                          index
                        }
                      >
                        <td>
                          <div className="fw-semibold text-primary">
                            {item.specimenCode ||
                              item.specimenId ||
                              '—'}
                          </div>
                        </td>

                        <td className="fw-semibold">
                          {item.patientName ||
                            '—'}
                        </td>

                        <td>
                          {item.testName ||
                            '—'}
                        </td>

                        <td>
                          {formatDateTime(
                            item.receivedAt
                          )}
                        </td>

                        <td>
                          <span
                            className={`badge ${workStatusClass(
                              item.status
                            )}`}
                          >
                            {workStatusText(
                              item.status
                            )}
                          </span>
                        </td>

                        <td className="text-end">
                          {String(
                            item.status ||
                              ''
                          ).toLowerCase() ===
                          'to_result' ? (
                            <Link
                              to={`/technician/nhap-ket-qua/${encodeURIComponent(
                                item.id ??
                                  ''
                              )}`}
                              className="btn btn-sm btn-primary"
                            >
                              Nhập kết quả
                            </Link>
                          ) : (
                            <Link
                              to="/technician/worklist"
                              className="btn btn-sm btn-outline-primary"
                            >
                              Xử lý
                            </Link>
                          )}
                        </td>
                      </tr>
                    )
                  )
                ) : (
                  <tr>
                    <td
                      colSpan="6"
                      className="text-center text-secondary py-5"
                    >
                      <i className="fa-solid fa-flask fs-2 d-block mb-3 opacity-50" />

                      Hiện không có worklist cần xử lý.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
}