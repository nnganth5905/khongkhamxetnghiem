import React, { useEffect, useMemo, useState } from 'react';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

// IMPORT service mới
import visitService from '../../services/visitService';
import { getApiErrorMessage } from '../../services/api';

// Map lại với trường dữ liệu backend trả về
const normalizeQueueItem = (item = {}) => ({
  id: item.maLuot || item.id || Math.random().toString(), // Dùng mã lượt làm ID tạm
  code: item.maLuot || '—',
  queueNumber: item.stt || '—',
  patientName: item.tenNguoiBenh || '—',
  service: item.dichVu || '—',
  room: item.phong || '—',
  checkInTime: item.checkInTime || '—',
  status: item.trangThai || 'WAITING',
});

const statusText = (status) => {
  switch (String(status || '').toLowerCase()) {
    case 'da_tiep_nhan':
      return 'Đã tiếp nhận';
    case 'cho_kham':
      return 'Chờ khám';
    case 'cho_xet_nghiem':
      return 'Chờ xét nghiệm';
    case 'waiting':
      return 'Đang chờ';
    case 'called':
      return 'Đã gọi';
    case 'examining':
    case 'dang_kham':
      return 'Đang khám';
    case 'testing':
    case 'dang_lay_mau':
      return 'Đang lấy mẫu';
    case 'completed':
    case 'hoan_tat':
      return 'Hoàn tất';
    default:
      return status || 'Chưa xác định';
  }
};

const statusClass = (status) => {
  switch (String(status || '').toLowerCase()) {
    case 'da_tiep_nhan':
      return 'bg-primary-subtle text-primary';
    case 'cho_kham':
    case 'cho_xet_nghiem':
    case 'called':
      return 'bg-warning-subtle text-warning-emphasis';
    case 'examining':
    case 'testing':
    case 'dang_kham':
    case 'dang_lay_mau':
      return 'bg-info-subtle text-info-emphasis';
    case 'completed':
    case 'hoan_tat':
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

  const loadQueue = async (showLoading = true) => {
    try {
      if (showLoading) {
        setLoading(true);
      }

      // SỬ DỤNG visitService GỌI API MỚI
      const data = await visitService.getReceptionWaitingList('ALL');

      const list = Array.isArray(data)
        ? data
        : data?.content || data?.items || data?.data || [];

      setQueue(list.map(normalizeQueueItem));
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
          <h1 className="dashboard-page-title mb-1" style={{ color: 'var(--primary, #0d6efd)' }}>
            Danh sách chờ
          </h1>

          <p className="text-secondary mb-0">
            Hàng chờ hiện tại của khách khám bệnh và xét nghiệm.
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-primary bg-white fw-medium"
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
              <span className="input-group-text bg-white border-end-0">
                <i className="fa-solid fa-magnifying-glass text-secondary" />
              </span>

              <input
                type="search"
                className="form-control border-start-0 ps-0"
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
              <table className="table table-borderless table-hover align-middle mb-0">
                <thead className="border-bottom" style={{ backgroundColor: '#fdfdfd' }}>
                  <tr>
                    <th className="py-3" style={{ width: 85 }}>
                      STT
                    </th>
                    <th className="py-3">Mã lượt</th>
                    <th className="py-3">Người bệnh</th>
                    <th className="py-3">Dịch vụ</th>
                    <th className="py-3">Phòng</th>
                    <th className="py-3">Check-in</th>
                    <th className="py-3">Trạng thái</th>
                  </tr>
                </thead>

                <tbody>
                  {filteredQueue.length > 0 ? (
                    filteredQueue.map((item, index) => (
                      <tr key={item.id} className="border-bottom">
                        <td>
                          <span className="fw-bold fs-6">
                            {item.queueNumber}
                          </span>
                        </td>

                        <td className="fw-semibold text-primary">
                          {item.code}
                        </td>

                        <td className="fw-bold">
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
                            className={`badge ${statusClass(item.status)} px-2 py-1`}
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