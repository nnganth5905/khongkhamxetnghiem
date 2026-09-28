import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import {
  useNavigate,
} from 'react-router-dom';

import Notification from '../../components/Notification';
import Loading from '../../components/Loading';

import {
  getPendingDoctorResults,
} from '../../services/doctorService';

import {
  getApiErrorMessage,
} from '../../services/api';

const formatDateTime =
  (value) => {
    if (!value) {
      return '—';
    }

    const text =
      String(value)
        .replace('T', ' ');

    const match =
      text.match(
        /^(\d{4})-(\d{2})-(\d{2})[ ](\d{2}):(\d{2})/,
      );

    if (!match) {
      return text;
    }

    return `${match[4]}:${match[5]} - ${match[3]}/${match[2]}/${match[1]}`;
  };

export default function DuyetKetQua() {
  const navigate =
    useNavigate();

  const [
    items,
    setItems,
  ] = useState([]);

  const [
    keyword,
    setKeyword,
  ] = useState('');

  const [
    loading,
    setLoading,
  ] = useState(true);

  const [
    error,
    setError,
  ] = useState('');

  const load =
    async () => {
      try {
        setLoading(
          true,
        );

        setError('');

        const data =
          await getPendingDoctorResults();

        setItems(
          Array.isArray(data)
            ? data
            : [],
        );
      } catch (err) {
        setError(
          getApiErrorMessage(
            err,
            'Không thể tải kết quả chờ duyệt.',
          ),
        );
      } finally {
        setLoading(
          false,
        );
      }
    };

  useEffect(() => {
    load();
  }, []);

  const filtered =
    useMemo(
      () => {
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
              item.id,
              item.specimenCode,
              item.patientName,
              item.testName,
              item.technicianName,
            ]
              .filter(Boolean)
              .join(' ')
              .toLowerCase()
              .includes(q),
        );
      },
      [
        items,
        keyword,
      ],
    );

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4 gap-3">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Duyệt kết quả xét nghiệm
          </h1>

          <p className="text-secondary mb-0">
            Các kết quả kỹ thuật viên đã gửi và đang chờ bác sĩ duyệt.
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-primary"
          onClick={
            load
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
          <div className="mb-4">
            <input
              type="search"
              className="form-control"
              placeholder="Tìm bệnh nhân, mẫu, xét nghiệm..."
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

          {loading ? (
            <Loading text="Đang tải kết quả..." />
          ) : (
            <div className="table-responsive">
              <table className="table table-hover align-middle">
                <thead className="table-light">
                  <tr>
                    <th>
                      #
                    </th>

                    <th>
                      Mã kết quả
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
                      KTV
                    </th>

                    <th>
                      Thời gian gửi
                    </th>

                    <th>
                      Cảnh báo
                    </th>

                    <th />
                  </tr>
                </thead>

                <tbody>
                  {filtered.length >
                  0 ? (
                    filtered.map(
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
                              item.specimenCode
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
                              item.technicianName
                            }
                          </td>

                          <td>
                            {formatDateTime(
                              item.submittedAt,
                            )}
                          </td>

                          <td>
                            {item.hasAbnormalIndicator ? (
                              <span className="badge bg-danger">
                                Có bất thường
                              </span>
                            ) : (
                              <span className="badge bg-success">
                                Không phát hiện cảnh báo
                              </span>
                            )}
                          </td>

                          <td className="text-end">
                            <button
                              type="button"
                              className="btn btn-sm btn-primary"
                              onClick={() =>
                                navigate(
                                  `/doctor/doc-ket-qua/${encodeURIComponent(
                                    item.id,
                                  )}`,
                                )
                              }
                            >
                              Xem & duyệt
                            </button>
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
                        Không có kết quả đang chờ duyệt.
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