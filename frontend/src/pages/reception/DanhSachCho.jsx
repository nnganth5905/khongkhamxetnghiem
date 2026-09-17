import React, { useEffect, useMemo, useState } from 'react';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import { getWaitingQueue } from '../../services/appointmentService';
import { getApiErrorMessage } from '../../services/api';

const normalizeQueueItem = (item = {}) => ({
  id:
    item.id ??
    item.idLuotKham ??
    item.IDLuotKham ??
    item.idLuotXetNghiem ??
    item.IDLuotXetNghiem,

  code:
    item.code ??
    item.maLuot ??
    item.MaLuot ??
    '—',

  queueNumber:
    item.queueNumber ??
    item.soThuTu ??
    item.SoThuTu,

  patientName:
    item.patientName ??
    item.tenKhachHang ??
    item.TenKhachHang ??
    '—',

  service:
    item.service ??
    item.serviceName ??
    item.dichVu ??
    item.tenDichVu ??
    '—',

  room:
    item.room ??
    item.roomName ??
    item.tenPhong ??
    item.TenPhong ??
    '—',

  checkInTime:
    item.checkInTime ??
    item.thoiGianCheckIn ??
    item.ThoiGianCheckIn ??
    '—',

  status:
    item.status ??
    item.trangThai ??
    item.TrangThai ??
    'WAITING',
});

const statusText = (status) => {
  switch (String(status || '').toUpperCase()) {
    case 'WAITING':
      return 'Đang chờ';

    case 'CALLED':
      return 'Đã gọi';

    case 'EXAMINING':
      return 'Đang khám';

    case 'TESTING':
      return 'Đang xét nghiệm';

    case 'COMPLETED':
      return 'Hoàn tất';

    default:
      return status || 'Chưa xác định';
  }
};

const statusClass = (status) => {
  switch (String(status || '').toUpperCase()) {
    case 'CALLED':
      return 'bg-warning-subtle text-warning-emphasis';

    case 'EXAMINING':
    case 'TESTING':
      return 'bg-primary-subtle text-primary';

    case 'COMPLETED':
      return 'bg-success-subtle text-success';

    default:
      return 'bg-secondary-subtle text-secondary';
  }
};

export default function DanhSachCho() {
  const [queue, setQueue] = useState([]);
  const [keyword, setKeyword] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const loadQueue = async (
    showLoading = true
  ) => {
    try {
      if (showLoading) {
        setLoading(true);
      }

      const data = await getWaitingQueue();

      const list = Array.isArray(data)
        ? data
        : data?.content ||
          data?.items ||
          data?.data ||
          [];

      setQueue(
        list.map(normalizeQueueItem)
      );
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          'Không thể tải danh sách chờ.'
        )
      );
    } finally {
      if (showLoading) {
        setLoading(false);
      }
    }
  };

  useEffect(() => {
    loadQueue();

    const timer = setInterval(() => {
      loadQueue(false);
    }, 30000);

    return () => {
      clearInterval(timer);
    };
  }, []);

  const filteredQueue = useMemo(() => {
    const q = keyword.trim().toLowerCase();

    if (!q) {
      return queue;
    }

    return queue.filter((item) =>
      [
        item.code,
        item.patientName,
        item.service,
        item.room,
      ]
        .join(' ')
        .toLowerCase()
        .includes(q)
    );
  }, [queue, keyword]);

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Danh sách chờ
          </h1>

          <p className="text-secondary mb-0">
            Hàng chờ hiện tại của khách khám bệnh và xét nghiệm.
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-primary"
          onClick={() => loadQueue()}
          disabled={loading}
        >
          <i className="fa-solid fa-rotate me-2" />
          Làm mới
        </button>
      </div>

      {error && (
        <Notification
          type="danger"
          message={error}
          onClose={() => setError('')}
        />
      )}

      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body p-4">
          <div className="d-flex flex-wrap gap-3 justify-content-between align-items-center mb-4">
            <div
              className="input-group"
              style={{
                maxWidth: 520,
              }}
            >
              <span className="input-group-text bg-white">
                <i className="fa-solid fa-magnifying-glass text-secondary" />
              </span>

              <input
                type="search"
                className="form-control"
                placeholder="Tìm mã lượt, tên người bệnh, dịch vụ..."
                value={keyword}
                onChange={(e) =>
                  setKeyword(
                    e.target.value
                  )
                }
              />
            </div>

            <div className="small text-secondary">
              Tự động cập nhật mỗi 30 giây
            </div>
          </div>

          {loading ? (
            <Loading text="Đang tải hàng chờ..." />
          ) : (
            <div className="table-responsive">
              <table className="table table-hover align-middle">
                <thead className="table-light">
                  <tr>
                    <th style={{ width: 85 }}>
                      STT
                    </th>

                    <th>
                      Mã lượt
                    </th>

                    <th>
                      Người bệnh
                    </th>

                    <th>
                      Dịch vụ
                    </th>

                    <th>
                      Phòng
                    </th>

                    <th>
                      Check-in
                    </th>

                    <th>
                      Trạng thái
                    </th>
                  </tr>
                </thead>

                <tbody>
                  {filteredQueue.length > 0 ? (
                    filteredQueue.map((item, index) => (
                      <tr key={item.id ?? index}>
                        <td>
                          <span className="badge bg-primary fs-6">
                            {item.queueNumber ??
                              index + 1}
                          </span>
                        </td>

                        <td className="fw-semibold">
                          {item.code}
                        </td>

                        <td>
                          {item.patientName}
                        </td>

                        <td>
                          {item.service}
                        </td>

                        <td>
                          {item.room}
                        </td>

                        <td>
                          {item.checkInTime}
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
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td
                        colSpan="7"
                        className="text-center text-secondary py-5"
                      >
                        Hàng chờ hiện đang trống.
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