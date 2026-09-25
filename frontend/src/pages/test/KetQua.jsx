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
const normalizeStatus = (status) => {
if (!status) {
return 'Đã duyệt';
}
const value = String(status).toLowerCase();
switch (value) {
case 'da_duyet':
return 'Đã duyệt';
case 'chua_duyet':
  return 'Chờ duyệt';

case 'dang_xu_ly':
  return 'Đang xử lý';

case 'hoan_tat':
  return 'Hoàn tất';

case 'completed':
  return 'Hoàn tất';

case 'approved':
  return 'Đã duyệt';

case 'pending':
  return 'Đang xử lý';

default:
  return status;
}
};
const normalizeResult = (item = {}) => {
const rawStatus =
item.status ??
item.trangThai ??
item.TrangThai ??
'da_duyet';
return {
id:
item.id ??
item.idKetQua ??
item.IDKetQua ??
item.code ??
item.maKetQua,
code:
  item.code ??
  item.id ??
  item.idKetQua ??
  item.IDKetQua ??
  item.maKetQua ??
  '—',

type:
  item.type ??
  item.loai ??
  'XÉT NGHIỆM',

testName:
  item.title ??
  item.testName ??
  item.tenXetNghiem ??
  item.TenXetNghiem ??
  '—',

testDate:
  item.date
    ? new Date(item.date).toLocaleString('vi-VN')
    : item.testDate
      ? new Date(item.testDate).toLocaleString('vi-VN')
      : item.ngayXetNghiem
        ? new Date(item.ngayXetNghiem).toLocaleString('vi-VN')
        : '—',

doctorName:
  item.doctorName ??
  item.tenBacSi ??
  item.TenBacSi ??
  '—',

summary:
  item.summary ??
  item.conclusion ??
  item.KetLuanBacSi ??
  '—',

rawStatus,

status:
  normalizeStatus(rawStatus),

abnormal:
  Boolean(
    item.abnormal ??
    item.coBatThuong ??
    false
  ),
};
};
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
  const data = await getMyResults();

  console.log(
    '[KetQua] API /results/mine:',
    data
  );

  let list = [];

  if (Array.isArray(data)) {
    list = data;
  } else if (
    data &&
    Array.isArray(data.items)
  ) {
    list = data.items;
  } else if (
    data &&
    Array.isArray(data.content)
  ) {
    list = data.content;
  } else if (
    data &&
    Array.isArray(data.data)
  ) {
    list = data.data;
  }

  const normalized = list.map(
    normalizeResult
  );

  console.log(
    '[KetQua] normalized results:',
    normalized
  );

  setResults(normalized);

} catch (err) {

  console.error(
    '[KetQua] load results error:',
    err
  );

  setError(
    getApiErrorMessage(
      err,
      'Không thể tải kết quả xét nghiệm.'
    )
  );

  setResults([]);

} finally {
  setLoading(false);
}
};
useEffect(() => {
loadResults();
}, []);
const filteredResults = useMemo(() => {

const q =
  keyword
    .trim()
    .toLowerCase();

return results.filter((item) => {

  const matchesKeyword =
    !q ||
    [
      item.code,
      item.testName,
      item.doctorName,
      item.type,
      item.summary,
    ]
      .join(' ')
      .toLowerCase()
      .includes(q);

  if (!matchesKeyword) {
    return false;
  }

  if (!status) {
    return true;
  }

  const raw =
    String(
      item.rawStatus ?? ''
    ).toLowerCase();

  if (status === 'PENDING') {
    return (
      raw === 'chua_duyet' ||
      raw === 'pending' ||
      raw === 'dang_xu_ly'
    );
  }

  if (status === 'APPROVED') {
    return (
      raw === 'da_duyet' ||
      raw === 'approved'
    );
  }

  if (status === 'COMPLETED') {
    return (
      raw === 'hoan_tat' ||
      raw === 'completed'
    );
  }

  return true;
});
}, [
results,
keyword,
status,
]);
return (

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
            color: 'var(--primary)',
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

          <Loading
            text="Đang tải kết quả..."
          />

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

                  filteredResults.map(
                    (item, index) => (

                      <tr
                        key={
                          item.id ??
                          index
                        }
                      >

                        <td>
                          {index + 1}
                        </td>

                        <td className="fw-semibold">

                          {item.code}

                        </td>

                        <td>

                          <div className="fw-semibold">
                            {item.testName}
                          </div>

                          <div className="small text-secondary">
                            {item.type}
                          </div>

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
                            to={`/ket-qua/${encodeURIComponent(
                              item.id
                            )}`}
                            className="btn btn-sm btn-primary"
                          >

                            <i className="fa-solid fa-eye me-2" />

                            Chi tiết

                          </Link>

                        </td>

                      </tr>

                    )
                  )

                ) : (

                  <tr>

                    <td
                      colSpan="8"
                      className="text-center text-secondary py-5"
                    >

                      {error
                        ? 'Không thể tải dữ liệu.'
                        : 'Chưa có kết quả xét nghiệm phù hợp.'}

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