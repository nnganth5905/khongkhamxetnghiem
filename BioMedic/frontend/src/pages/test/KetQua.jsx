import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import { Link } from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import { getMyResults } from '../../services/testService';
import { getApiErrorMessage } from '../../services/api';

const normalizeResult = (item = {}) => ({
  id:
    item.id ??
    item.idKetQua ??
    item.IDKetQua,

  code:
    item.code ??
    item.maKetQua ??
    item.MaKetQua ??
    item.maPhieu ??
    item.MaPhieu ??
    '—',

  testName:
    item.testName ??
    item.tenXetNghiem ??
    item.TenXetNghiem ??
    '—',

  testDate:
    item.testDate ??
    item.ngayXetNghiem ??
    item.NgayXetNghiem ??
    '—',

  doctorName:
    item.doctorName ??
    item.tenBacSi ??
    item.TenBacSi ??
    '—',

  status:
    item.status ??
    item.trangThai ??
    item.TrangThai ??
    '—',

  abnormal: Boolean(
    item.abnormal ??
    item.coBatThuong ??
    item.CoBatThuong ??
    false
  ),
});

export default function KetQua() {
  const [results, setResults] = useState([]);
  const [keyword, setKeyword] = useState('');
  const [status, setStatus] = useState('');

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const loadResults = async () => {
    try {
      setLoading(true);
      setError('');

      const data =
        await getMyResults({
          status:
            status || undefined,
        });

      const list = Array.isArray(data)
        ? data
        : data?.content ||
          data?.items ||
          data?.data ||
          [];

      setResults(
        list.map(normalizeResult)
      );
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          'Không thể tải kết quả xét nghiệm.'
        )
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadResults();
  }, [status]);

  const filteredResults =
    useMemo(() => {
      const q =
        keyword
          .trim()
          .toLowerCase();

      if (!q) {
        return results;
      }

      return results.filter((item) =>
        [
          item.code,
          item.testName,
          item.doctorName,
        ]
          .join(' ')
          .toLowerCase()
          .includes(q)
      );
    }, [results, keyword]);

  return (
    <div className="bg-light min-vh-100 py-5">
      <div
        className="container"
        style={{
          maxWidth: 1100,
        }}
      >
        <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
          <div>
            <h1
              className="fw-bold mb-1"
              style={{
                color:
                  'var(--primary)',
              }}
            >
              Kết quả xét nghiệm
            </h1>

            <p className="text-secondary mb-0">
              Xem các kết quả xét nghiệm đã được trả trên hệ thống.
            </p>
          </div>

          <Link
            to="/tra-cuu-ket-qua"
            className="btn btn-outline-primary"
          >
            <i className="fa-solid fa-magnifying-glass me-2" />
            Tra cứu không đăng nhập
          </Link>
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
                  placeholder="Tìm mã kết quả, tên xét nghiệm, bác sĩ..."
                  value={keyword}
                  onChange={(e) =>
                    setKeyword(
                      e.target.value
                    )
                  }
                />
              </div>

              <div className="col-lg-4">
                <select
                  className="form-select"
                  value={status}
                  onChange={(e) =>
                    setStatus(
                      e.target.value
                    )
                  }
                >
                  <option value="">
                    Tất cả trạng thái
                  </option>

                  <option value="PENDING">
                    Đang xử lý
                  </option>

                  <option value="APPROVED">
                    Đã duyệt
                  </option>

                  <option value="COMPLETED">
                    Hoàn tất
                  </option>
                </select>
              </div>
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
                        Xét nghiệm
                      </th>

                      <th>
                        Ngày xét nghiệm
                      </th>

                      <th>
                        Bác sĩ
                      </th>

                      <th>
                        Đánh giá
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
                    {filteredResults.length > 0 ? (
                      filteredResults.map((item, index) => (
                        <tr key={item.id ?? index}>
                          <td>
                            {index + 1}
                          </td>

                          <td className="fw-semibold">
                            {item.code}
                          </td>

                          <td>
                            {item.testName}
                          </td>

                          <td>
                            {item.testDate}
                          </td>

                          <td>
                            {item.doctorName}
                          </td>

                          <td>
                            {item.abnormal ? (
                              <span className="badge bg-danger-subtle text-danger">
                                Có bất thường
                              </span>
                            ) : (
                              <span className="badge bg-success-subtle text-success">
                                Trong ngưỡng
                              </span>
                            )}
                          </td>

                          <td>
                            <span className="badge bg-primary-subtle text-primary">
                              {item.status}
                            </span>
                          </td>

                          <td className="text-end">
                            <Link
                              to={`/ket-qua/${item.id}`}
                              className="btn btn-sm btn-primary"
                            >
                              <i className="fa-solid fa-eye me-2" />
                              Chi tiết
                            </Link>
                          </td>
                        </tr>
                      ))
                    ) : (
                      <tr>
                        <td
                          colSpan="8"
                          className="text-center text-secondary py-5"
                        >
                          Chưa có kết quả xét nghiệm phù hợp.
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
    </div>
  );
}