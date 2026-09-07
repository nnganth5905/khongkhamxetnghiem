import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import { getTrackingHistory } from '../../services/trackingService';
import { getApiErrorMessage } from '../../services/api';

const normalizeItem = (item = {}) => ({
  id:
    item.id ??
    item.idTruyVet ??
    item.IDTruyVet,

  createdAt:
    item.createdAt ??
    item.thoiGian ??
    item.ThoiGian ??
    item.ngayTao ??
    '—',

  entityType:
    item.entityType ??
    item.loaiDoiTuong ??
    item.LoaiDoiTuong ??
    '—',

  entityCode:
    item.entityCode ??
    item.maDoiTuong ??
    item.MaDoiTuong ??
    '—',

  action:
    item.action ??
    item.hanhDong ??
    item.HanhDong ??
    '—',

  actorName:
    item.actorName ??
    item.nguoiThucHien ??
    item.NguoiThucHien ??
    '—',

  note:
    item.note ??
    item.ghiChu ??
    item.GhiChu ??
    '—',
});

export default function LichSuTruyVet() {
  const [items, setItems] = useState([]);

  const [keyword, setKeyword] = useState('');
  const [entityType, setEntityType] = useState('');

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const loadHistory = async () => {
    try {
      setLoading(true);
      setError('');

      const data =
        await getTrackingHistory({
          entityType:
            entityType || undefined,
        });

      const list = Array.isArray(data)
        ? data
        : data?.content ||
          data?.items ||
          data?.data ||
          [];

      setItems(
        list.map(normalizeItem)
      );
    } catch (err) {
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
  }, [entityType]);

  const filteredItems = useMemo(() => {
    const q = keyword.trim().toLowerCase();

    if (!q) {
      return items;
    }

    return items.filter((item) =>
      [
        item.entityType,
        item.entityCode,
        item.action,
        item.actorName,
        item.note,
      ]
        .join(' ')
        .toLowerCase()
        .includes(q)
    );
  }, [items, keyword]);

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Lịch sử truy vết
          </h1>

          <p className="text-secondary mb-0">
            Theo dõi lịch sử thay đổi của lượt khám, phiếu xét nghiệm, mẫu và kết quả.
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-primary"
          onClick={loadHistory}
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
          <div className="row g-3 mb-4">
            <div className="col-lg-8">
              <input
                type="search"
                className="form-control"
                placeholder="Tìm mã đối tượng, hành động, người thực hiện..."
                value={keyword}
                onChange={(e) => setKeyword(e.target.value)}
              />
            </div>

            <div className="col-lg-4">
              <select
                className="form-select"
                value={entityType}
                onChange={(e) =>
                  setEntityType(
                    e.target.value
                  )
                }
              >
                <option value="">
                  Tất cả đối tượng
                </option>

                <option value="VISIT">
                  Lượt khám
                </option>

                <option value="TEST_VISIT">
                  Lượt xét nghiệm
                </option>

                <option value="TEST_ORDER">
                  Phiếu xét nghiệm
                </option>

                <option value="SPECIMEN">
                  Mẫu bệnh phẩm
                </option>

                <option value="RESULT">
                  Kết quả xét nghiệm
                </option>
              </select>
            </div>
          </div>

          {loading ? (
            <Loading text="Đang tải lịch sử truy vết..." />
          ) : (
            <div className="table-responsive">
              <table className="table table-hover align-middle">
                <thead className="table-light">
                  <tr>
                    <th>
                      Thời gian
                    </th>

                    <th>
                      Loại đối tượng
                    </th>

                    <th>
                      Mã đối tượng
                    </th>

                    <th>
                      Hành động
                    </th>

                    <th>
                      Người thực hiện
                    </th>

                    <th>
                      Ghi chú
                    </th>
                  </tr>
                </thead>

                <tbody>
                  {filteredItems.length > 0 ? (
                    filteredItems.map((item, index) => (
                      <tr key={item.id ?? index}>
                        <td>
                          {item.createdAt}
                        </td>

                        <td>
                          <span className="badge bg-primary-subtle text-primary">
                            {item.entityType}
                          </span>
                        </td>

                        <td className="fw-semibold">
                          {item.entityCode}
                        </td>

                        <td>
                          {item.action}
                        </td>

                        <td>
                          {item.actorName}
                        </td>

                        <td>
                          {item.note}
                        </td>
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td
                        colSpan="6"
                        className="text-center text-secondary py-5"
                      >
                        Chưa có dữ liệu truy vết phù hợp.
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