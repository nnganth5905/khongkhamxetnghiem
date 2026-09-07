import React, { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import {
  completeWork,
  getWorklist,
  startWork,
} from '../../services/technicianService';

import { getApiErrorMessage } from '../../services/api';

const normalizeWorkItem = (item = {}) => ({
  id:
    item.id ??
    item.idWorklist ??
    item.IDWorklist,

  specimenCode:
    item.specimenCode ??
    item.maMau ??
    item.MaMau ??
    '—',

  testName:
    item.testName ??
    item.tenXetNghiem ??
    item.TenXetNghiem ??
    '—',

  patientName:
    item.patientName ??
    item.tenKhachHang ??
    item.TenKhachHang ??
    '—',

  priority:
    item.priority ??
    item.uuTien ??
    item.UuTien ??
    'NORMAL',

  assignedAt:
    item.assignedAt ??
    item.thoiGianPhanCong ??
    item.ThoiGianPhanCong ??
    '—',

  status:
    item.status ??
    item.trangThai ??
    item.TrangThai ??
    'PENDING',
});

const statusText = (status) => {
  switch (String(status || '').toUpperCase()) {
    case 'PENDING':
      return 'Chờ thực hiện';

    case 'IN_PROGRESS':
      return 'Đang thực hiện';

    case 'COMPLETED':
      return 'Đã thực hiện';

    case 'RESULT_ENTERED':
      return 'Đã nhập kết quả';

    case 'SUBMITTED':
      return 'Đã gửi duyệt';

    default:
      return status || 'Chưa xác định';
  }
};

const statusClass = (status) => {
  switch (String(status || '').toUpperCase()) {
    case 'IN_PROGRESS':
      return 'bg-warning-subtle text-warning-emphasis';

    case 'COMPLETED':
    case 'RESULT_ENTERED':
      return 'bg-success-subtle text-success';

    case 'SUBMITTED':
      return 'bg-primary-subtle text-primary';

    default:
      return 'bg-secondary-subtle text-secondary';
  }
};

export default function ThucHienXetNghiem() {
  const [worklist, setWorklist] = useState([]);
  const [keyword, setKeyword] = useState('');
  const [status, setStatus] = useState('');
  const [loading, setLoading] = useState(true);
  const [processingId, setProcessingId] = useState(null);

  const [message, setMessage] = useState({
    type: '',
    text: '',
  });

  const loadWorklist = async () => {
    try {
      setLoading(true);

      const data = await getWorklist({
        status: status || undefined,
      });

      const list = Array.isArray(data)
        ? data
        : data?.content ||
          data?.items ||
          data?.data ||
          [];

      setWorklist(
        list.map(normalizeWorkItem)
      );
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(
          err,
          'Không thể tải worklist xét nghiệm.'
        ),
      });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadWorklist();
  }, [status]);

  const filteredWorklist = useMemo(() => {
    const q = keyword.trim().toLowerCase();

    if (!q) {
      return worklist;
    }

    return worklist.filter((item) =>
      [
        item.specimenCode,
        item.testName,
        item.patientName,
      ]
        .join(' ')
        .toLowerCase()
        .includes(q)
    );
  }, [worklist, keyword]);

  const handleStart = async (item) => {
    try {
      setProcessingId(item.id);

      await startWork(item.id);

      setMessage({
        type: 'success',
        text: 'Đã bắt đầu thực hiện xét nghiệm.',
      });

      await loadWorklist();
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(
          err,
          'Không thể bắt đầu xét nghiệm.'
        ),
      });
    } finally {
      setProcessingId(null);
    }
  };

  const handleComplete = async (item) => {
    const confirmed = window.confirm(
      'Xác nhận đã hoàn tất bước thực hiện xét nghiệm?'
    );

    if (!confirmed) {
      return;
    }

    try {
      setProcessingId(item.id);

      await completeWork(item.id);

      setMessage({
        type: 'success',
        text: 'Đã hoàn tất bước thực hiện xét nghiệm.',
      });

      await loadWorklist();
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(
          err,
          'Không thể hoàn tất xét nghiệm.'
        ),
      });
    } finally {
      setProcessingId(null);
    }
  };

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Thực hiện xét nghiệm
          </h1>

          <p className="text-secondary mb-0">
            Quản lý worklist, bắt đầu xét nghiệm và chuyển sang bước nhập kết quả.
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-primary"
          onClick={loadWorklist}
          disabled={loading}
        >
          <i className="fa-solid fa-rotate me-2" />
          Làm mới
        </button>
      </div>

      {message.text && (
        <Notification
          type={message.type}
          message={message.text}
          onClose={() =>
            setMessage({
              type: '',
              text: '',
            })
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
                placeholder="Tìm theo mã mẫu, xét nghiệm hoặc người bệnh..."
                value={keyword}
                onChange={(e) => setKeyword(e.target.value)}
              />
            </div>

            <div className="col-lg-4">
              <select
                className="form-select"
                value={status}
                onChange={(e) => setStatus(e.target.value)}
              >
                <option value="">
                  Tất cả trạng thái
                </option>

                <option value="PENDING">
                  Chờ thực hiện
                </option>

                <option value="IN_PROGRESS">
                  Đang thực hiện
                </option>

                <option value="COMPLETED">
                  Đã thực hiện
                </option>

                <option value="RESULT_ENTERED">
                  Đã nhập kết quả
                </option>
              </select>
            </div>
          </div>

          {loading ? (
            <Loading text="Đang tải worklist..." />
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
                      Xét nghiệm
                    </th>

                    <th>
                      Người bệnh
                    </th>

                    <th>
                      Ưu tiên
                    </th>

                    <th>
                      Phân công lúc
                    </th>

                    <th>
                      Trạng thái
                    </th>

                    <th className="text-end">
                      Thao tác
                    </th>
                  </tr>
                </thead>

                <tbody>
                  {filteredWorklist.length > 0 ? (
                    filteredWorklist.map((item, index) => (
                      <tr key={item.id ?? index}>
                        <td>
                          {index + 1}
                        </td>

                        <td className="fw-semibold">
                          {item.specimenCode}
                        </td>

                        <td>
                          {item.testName}
                        </td>

                        <td>
                          {item.patientName}
                        </td>

                        <td>
                          <span
                            className={`badge ${
                              String(item.priority).toUpperCase() === 'URGENT'
                                ? 'bg-danger'
                                : 'bg-secondary'
                            }`}
                          >
                            {String(item.priority).toUpperCase() === 'URGENT'
                              ? 'Khẩn'
                              : 'Bình thường'}
                          </span>
                        </td>

                        <td>
                          {item.assignedAt}
                        </td>

                        <td>
                          <span
                            className={`badge ${statusClass(
                              item.status
                            )}`}
                          >
                            {statusText(item.status)}
                          </span>
                        </td>

                        <td className="text-end">
                          {String(item.status).toUpperCase() === 'PENDING' && (
                            <button
                              type="button"
                              className="btn btn-sm btn-primary me-2"
                              disabled={processingId === item.id}
                              onClick={() => handleStart(item)}
                            >
                              <i className="fa-solid fa-play me-2" />
                              Bắt đầu
                            </button>
                          )}

                          {String(item.status).toUpperCase() === 'IN_PROGRESS' && (
                            <button
                              type="button"
                              className="btn btn-sm btn-success me-2"
                              disabled={processingId === item.id}
                              onClick={() => handleComplete(item)}
                            >
                              <i className="fa-solid fa-check me-2" />
                              Hoàn tất
                            </button>
                          )}

                          {[
                            'IN_PROGRESS',
                            'COMPLETED',
                            'RESULT_ENTERED',
                          ].includes(
                            String(item.status).toUpperCase()
                          ) && (
                            <Link
                              to={`/technician/nhap-ket-qua/${item.id}`}
                              className="btn btn-sm btn-outline-primary"
                            >
                              <i className="fa-solid fa-file-pen me-2" />
                              Nhập kết quả
                            </Link>
                          )}
                        </td>
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td
                        colSpan="8"
                        className="text-center text-secondary py-5"
                      >
                        Chưa có công việc xét nghiệm phù hợp.
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