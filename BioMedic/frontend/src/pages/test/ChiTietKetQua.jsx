import React, {
  useEffect,
  useState,
} from 'react';

import {
  Link,
  useParams,
} from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import { getResult } from '../../services/testService';
import { getApiErrorMessage } from '../../services/api';

const formatValue = (value) => {
  if (
    value === null ||
    value === undefined ||
    value === ''
  ) {
    return '—';
  }

  return value;
};

export default function ChiTietKetQua() {
  const { resultId } = useParams();

  const queryId =
    new URLSearchParams(
      window.location.search
    ).get('id');

  const id =
    resultId || queryId;

  const [result, setResult] = useState(null);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let active = true;

    const loadResult = async () => {
      if (!id) {
        setError(
          'Thiếu mã kết quả xét nghiệm.'
        );

        setLoading(false);
        return;
      }

      try {
        const data =
          await getResult(id);

        if (active) {
          setResult(data);
        }
      } catch (err) {
        if (active) {
          setError(
            getApiErrorMessage(
              err,
              'Không thể tải chi tiết kết quả.'
            )
          );
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    };

    loadResult();

    return () => {
      active = false;
    };
  }, [id]);

  if (loading) {
    return (
      <Loading text="Đang tải chi tiết kết quả..." />
    );
  }

  const indicators =
    result?.indicators ??
    result?.chiSo ??
    result?.details ??
    [];

  return (
    <div className="bg-light min-vh-100 py-5">
      <div
        className="container"
        style={{
          maxWidth: 1000,
        }}
      >
        {error && (
          <Notification
            type="danger"
            message={error}
          />
        )}

        {result && (
          <>
            <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
              <div>
                <h1
                  className="fw-bold mb-1"
                  style={{
                    color:
                      'var(--primary)',
                  }}
                >
                  Chi tiết kết quả
                </h1>

                <div className="text-secondary">
                  {result.code ??
                    result.maKetQua ??
                    result.MaKetQua ??
                    `#${id}`}
                </div>
              </div>

              <Link
                to="/ket-qua"
                className="btn btn-outline-secondary"
              >
                <i className="fa-solid fa-arrow-left me-2" />
                Quay lại
              </Link>
            </div>

            <div className="card border-0 shadow-sm rounded-4 mb-4">
              <div className="card-body p-4">
                <div className="row g-4">
                  <div className="col-md-4">
                    <div className="small text-secondary">
                      Người bệnh
                    </div>

                    <div className="fw-semibold">
                      {result.patientName ??
                        result.tenKhachHang ??
                        result.TenKhachHang ??
                        '—'}
                    </div>
                  </div>

                  <div className="col-md-4">
                    <div className="small text-secondary">
                      Ngày xét nghiệm
                    </div>

                    <div className="fw-semibold">
                      {result.testDate ??
                        result.ngayXetNghiem ??
                        result.NgayXetNghiem ??
                        '—'}
                    </div>
                  </div>

                  <div className="col-md-4">
                    <div className="small text-secondary">
                      Bác sĩ duyệt
                    </div>

                    <div className="fw-semibold">
                      {result.doctorName ??
                        result.tenBacSi ??
                        result.TenBacSi ??
                        '—'}
                    </div>
                  </div>

                  <div className="col-md-4">
                    <div className="small text-secondary">
                      Mã phiếu xét nghiệm
                    </div>

                    <div className="fw-semibold">
                      {result.orderCode ??
                        result.maPhieu ??
                        result.MaPhieu ??
                        '—'}
                    </div>
                  </div>

                  <div className="col-md-4">
                    <div className="small text-secondary">
                      Mã mẫu
                    </div>

                    <div className="fw-semibold">
                      {result.specimenCode ??
                        result.maMau ??
                        result.MaMau ??
                        '—'}
                    </div>
                  </div>

                  <div className="col-md-4">
                    <div className="small text-secondary">
                      Trạng thái
                    </div>

                    <span className="badge bg-success mt-1">
                      {result.status ??
                        result.trangThai ??
                        result.TrangThai ??
                        'Hoàn tất'}
                    </span>
                  </div>
                </div>
              </div>
            </div>

            <div className="card border-0 shadow-sm rounded-4 mb-4">
              <div className="card-body p-4">
                <h5 className="fw-bold mb-4">
                  Chỉ số xét nghiệm
                </h5>

                <div className="table-responsive">
                  <table className="table align-middle">
                    <thead className="table-light">
                      <tr>
                        <th>
                          Chỉ số
                        </th>

                        <th>
                          Kết quả
                        </th>

                        <th>
                          Đơn vị
                        </th>

                        <th>
                          Khoảng tham chiếu
                        </th>

                        <th>
                          Đánh giá
                        </th>
                      </tr>
                    </thead>

                    <tbody>
                      {indicators.length > 0 ? (
                        indicators.map((item, index) => {
                          const abnormal =
                            Boolean(
                              item.abnormal ??
                              item.batThuong ??
                              false
                            );

                          return (
                            <tr
                              key={
                                item.id ??
                                item.idChiSo ??
                                index
                              }
                            >
                              <td className="fw-semibold">
                                {item.name ??
                                  item.tenChiSo ??
                                  item.TenChiSo ??
                                  '—'}
                              </td>

                              <td>
                                {formatValue(
                                  item.value ??
                                  item.giaTri ??
                                  item.GiaTri
                                )}
                              </td>

                              <td>
                                {formatValue(
                                  item.unit ??
                                  item.donVi ??
                                  item.DonVi
                                )}
                              </td>

                              <td>
                                {formatValue(
                                  item.reference ??
                                  item.nguongThamChieu ??
                                  item.NguongThamChieu
                                )}
                              </td>

                              <td>
                                {abnormal ? (
                                  <span className="badge bg-danger">
                                    Bất thường
                                  </span>
                                ) : (
                                  <span className="badge bg-success">
                                    Bình thường
                                  </span>
                                )}
                              </td>
                            </tr>
                          );
                        })
                      ) : (
                        <tr>
                          <td
                            colSpan="5"
                            className="text-center text-secondary py-4"
                          >
                            Chưa có dữ liệu chỉ số.
                          </td>
                        </tr>
                      )}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            <div className="card border-0 shadow-sm rounded-4">
              <div className="card-body p-4">
                <h5 className="fw-bold mb-3">
                  Kết luận bác sĩ
                </h5>

                <div
                  className="mb-4"
                  style={{
                    whiteSpace:
                      'pre-line',
                    lineHeight:
                      1.8,
                  }}
                >
                  {result.doctorConclusion ??
                    result.ketLuanBacSi ??
                    result.KetLuanBacSi ??
                    'Chưa có kết luận.'}
                </div>

                {(result.advice ??
                  result.loiDan ??
                  result.LoiDan) && (
                  <>
                    <h6 className="fw-bold">
                      Lời dặn
                    </h6>

                    <div
                      style={{
                        whiteSpace:
                          'pre-line',
                        lineHeight:
                          1.8,
                      }}
                    >
                      {result.advice ??
                        result.loiDan ??
                        result.LoiDan}
                    </div>
                  </>
                )}
              </div>
            </div>
          </>
        )}
      </div>
    </div>
  );
}