import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import {
  Link,
} from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import {
  getSpecimens,
} from '../../services/technicianService';

import {
  getApiErrorMessage,
} from '../../services/api';

const getStatusClass =
  (status) => {
    switch (
      String(
        status ||
        '',
      ).toUpperCase()
    ) {
      case 'HANDED_OVER':
        return 'bg-warning-subtle text-warning-emphasis';

      case 'RECEIVED':
        return 'bg-success-subtle text-success';

      case 'REJECTED':
        return 'bg-danger-subtle text-danger';

      case 'PROCESSING':
      case 'IN_PROGRESS':
        return 'bg-info-subtle text-info-emphasis';

      case 'COMPLETED':
        return 'bg-primary-subtle text-primary';

      default:
        return 'bg-secondary-subtle text-secondary';
    }
  };

const getStatusText =
  (status) => {
    switch (
      String(
        status ||
        '',
      ).toUpperCase()
    ) {
      case 'HANDED_OVER':
        return 'Chờ tiếp nhận';

      case 'RECEIVED':
        return 'Đã tiếp nhận';

      case 'REJECTED':
        return 'Đã từ chối';

      case 'PROCESSING':
      case 'IN_PROGRESS':
        return 'Đang xử lý';

      case 'COMPLETED':
        return 'Hoàn tất';

      default:
        return (
          status ||
          'Chưa xác định'
        );
    }
  };

const formatDateTime =
  (value) => {
    if (!value) {
      return '—';
    }

    const date =
      new Date(value);

    if (
      Number.isNaN(
        date.getTime(),
      )
    ) {
      return value;
    }

    return date.toLocaleString(
      'vi-VN',
    );
  };

export default function DanhSachMau() {
  const [
    specimens,
    setSpecimens,
  ] = useState([]);

  const [
    keyword,
    setKeyword,
  ] = useState('');

  const [
    status,
    setStatus,
  ] = useState('');

  const [
    loading,
    setLoading,
  ] = useState(true);

  const [
    error,
    setError,
  ] = useState('');

  const loadSpecimens =
    async () => {
      try {
        setLoading(
          true,
        );

        setError('');

        const data =
          await getSpecimens({
            status:
              status ||
              undefined,
          });

        setSpecimens(
          Array.isArray(data)
            ? data
            : [],
        );
      } catch (err) {
        setError(
          getApiErrorMessage(
            err,
            'Không thể tải danh sách mẫu bệnh phẩm.',
          ),
        );
      } finally {
        setLoading(
          false,
        );
      }
    };

  useEffect(() => {
    loadSpecimens();
  }, [status]);

  const filteredSpecimens =
    useMemo(
      () => {
        const q =
          keyword
            .trim()
            .toLowerCase();

        if (!q) {
          return specimens;
        }

        return specimens.filter(
          (item) =>
            [
              item.id,
              item.barcode,
              item.patientName,
              item.specimenType,
              item.testName,
            ]
              .filter(Boolean)
              .join(' ')
              .toLowerCase()
              .includes(q),
        );
      },
      [
        specimens,
        keyword,
      ],
    );

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Danh sách mẫu bệnh phẩm
          </h1>

          <p className="text-secondary mb-0">
            Các mẫu được bàn giao trực tiếp cho kỹ thuật viên đang đăng nhập.
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-primary"
          onClick={
            loadSpecimens
          }
          disabled={
            loading
          }
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
          <div className="row g-3 mb-4">
            <div className="col-lg-8">
              <input
                type="search"
                className="form-control"
                placeholder="Tìm mã mẫu, barcode, tên bệnh nhân, xét nghiệm..."
                value={
                  keyword
                }
                onChange={(e) =>
                  setKeyword(
                    e.target.value,
                  )
                }
              />
            </div>

            <div className="col-lg-4">
              <select
                className="form-select"
                value={
                  status
                }
                onChange={(e) =>
                  setStatus(
                    e.target.value,
                  )
                }
              >
                <option value="">
                  Tất cả trạng thái
                </option>

                <option value="HANDED_OVER">
                  Chờ tiếp nhận
                </option>

                <option value="RECEIVED">
                  Đã tiếp nhận
                </option>

                <option value="REJECTED">
                  Đã từ chối
                </option>

                <option value="IN_PROGRESS">
                  Đang xử lý
                </option>

                <option value="COMPLETED">
                  Hoàn tất
                </option>
              </select>
            </div>
          </div>

          {loading ? (
            <Loading text="Đang tải danh sách mẫu..." />
          ) : (
            <div className="table-responsive">
              <table className="table table-hover align-middle">
                <thead className="table-light">
                  <tr>
                    <th>
                      #
                    </th>

                    <th>
                      Mã mẫu
                    </th>

                    <th>
                      Barcode
                    </th>

                    <th>
                      Người bệnh
                    </th>

                    <th>
                      Xét nghiệm
                    </th>

                    <th>
                      Loại mẫu
                    </th>

                    <th>
                      Thời gian lấy
                    </th>

                    <th>
                      Trạng thái
                    </th>

                    <th />
                  </tr>
                </thead>

                <tbody>
                  {filteredSpecimens.length >
                  0 ? (
                    filteredSpecimens.map(
                      (
                        item,
                        index,
                      ) => (
                        <tr
                          key={
                            item.id
                          }
                        >
                          <td>
                            {index +
                              1}
                          </td>

                          <td className="fw-semibold text-primary">
                            {
                              item.id
                            }
                          </td>

                          <td>
                            {
                              item.barcode
                            }
                          </td>

                          <td>
                            {
                              item.patientName
                            }
                          </td>

                          <td>
                            {
                              item.testName
                            }
                          </td>

                          <td>
                            {
                              item.specimenType
                            }
                          </td>

                          <td>
                            {formatDateTime(
                              item.collectedAt,
                            )}
                          </td>

                          <td>
                            <span
                              className={`badge ${getStatusClass(
                                item.status,
                              )}`}
                            >
                              {getStatusText(
                                item.status,
                              )}
                            </span>
                          </td>

                          <td className="text-end">
                            <Link
                              to={`/technician/tiep-nhan-mau/${encodeURIComponent(
                                item.id,
                              )}`}
                              className="btn btn-sm btn-primary"
                            >
                              Xem mẫu
                            </Link>
                          </td>
                        </tr>
                      ),
                    )
                  ) : (
                    <tr>
                      <td
                        colSpan="9"
                        className="text-center text-secondary py-5"
                      >
                        Không có mẫu phù hợp.
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