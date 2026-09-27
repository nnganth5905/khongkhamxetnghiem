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

import { getTest } from '../../services/testService';
import { getApiErrorMessage } from '../../services/api';

const formatVnd = (value) => {
  if (
    value === null ||
    value === undefined ||
    value === ''
  ) {
    return 'Liên hệ';
  }

  const number = Number(value);

  if (!Number.isFinite(number)) {
    return String(value);
  }

  return (
    number.toLocaleString('vi-VN') +
    ' đ'
  );
};

export default function ChiTietXetNghiemKDN() {
  const { testId } = useParams();

  const queryId =
    new URLSearchParams(
      window.location.search
    ).get('id');

  const id =
    testId || queryId;

  const [test, setTest] = useState(null);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let active = true;

    const loadTest = async () => {
      if (!id) {
        setError(
          'Thiếu mã xét nghiệm.'
        );

        setLoading(false);
        return;
      }

      try {
        const data =
          await getTest(id);

        if (active) {
          setTest(data);
        }
      } catch (err) {
        if (active) {
          setError(
            getApiErrorMessage(
              err,
              'Không thể tải thông tin xét nghiệm.'
            )
          );
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    };

    loadTest();

    return () => {
      active = false;
    };
  }, [id]);

  if (loading) {
    return (
      <Loading text="Đang tải thông tin xét nghiệm..." />
    );
  }

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

        {test && (
          <div className="card border-0 shadow-sm rounded-4 overflow-hidden">
            <div className="row g-0">
              <div className="col-lg-5">
                <img
                  src={
                    test.image ??
                    test.hinhAnh ??
                    test.HinhAnh ??
                    'https://placehold.co/900x900/eaf2ff/0360d9?text=Xet+nghiem'
                  }
                  alt={
                    test.name ??
                    test.tenXetNghiem ??
                    test.TenXetNghiem
                  }
                  className="w-100 h-100"
                  style={{
                    minHeight:
                      430,
                    objectFit:
                      'cover',
                  }}
                />
              </div>

              <div className="col-lg-7">
                <div className="p-4 p-lg-5">
                  <div className="small text-uppercase fw-semibold text-primary mb-2">
                    {test.category ??
                      test.tenLoai ??
                      test.TenLoai ??
                      'Dịch vụ xét nghiệm'}
                  </div>

                  <h1 className="fw-bold mb-3">
                    {test.name ??
                      test.tenXetNghiem ??
                      test.TenXetNghiem}
                  </h1>

                  <p
                    className="text-secondary"
                    style={{
                      lineHeight:
                        1.8,
                      whiteSpace:
                        'pre-line',
                    }}
                  >
                    {test.description ??
                      test.moTa ??
                      test.MoTa ??
                      'Thông tin xét nghiệm đang được cập nhật.'}
                  </p>

                  <div className="row g-3 my-4">
                    <div className="col-sm-6">
                      <div className="small text-secondary">
                        Loại mẫu
                      </div>

                      <div className="fw-semibold">
                        {test.sampleType ??
                          test.loaiMau ??
                          test.LoaiMau ??
                          'Theo hướng dẫn'}
                      </div>
                    </div>

                    <div className="col-sm-6">
                      <div className="small text-secondary">
                        Thời gian trả kết quả
                      </div>

                      <div className="fw-semibold">
                        {test.turnaround ??
                          test.thoiGianTraKetQua ??
                          test.ThoiGianTraKetQua ??
                          'Theo quy trình'}
                      </div>
                    </div>

                    <div className="col-sm-6">
                      <div className="small text-secondary">
                        Chuẩn bị trước xét nghiệm
                      </div>

                      <div className="fw-semibold">
                        {test.preparation ??
                          test.chuanBi ??
                          test.ChuanBi ??
                          'Theo hướng dẫn của nhân viên y tế'}
                      </div>
                    </div>

                    <div className="col-sm-6">
                      <div className="small text-secondary">
                        Cơ sở thực hiện
                      </div>

                      <div className="fw-semibold">
                        {test.facilityName ??
                          test.tenCoSo ??
                          test.TenCoSo ??
                          'Bio Medic Center'}
                      </div>
                    </div>
                  </div>

                  <div
                    className="fs-3 fw-bold mb-4"
                    style={{
                      color:
                        '#ff6a00',
                    }}
                  >
                    {formatVnd(
                      test.price ??
                      test.gia ??
                      test.Gia
                    )}
                  </div>

                  <div className="d-flex flex-wrap gap-2">
                    <Link
                      to={`/dat-lich-xet-nghiem?test=${encodeURIComponent(
                        id
                      )}`}
                      className="btn btn-primary px-4"
                    >
                      <i className="fa-regular fa-calendar-check me-2" />
                      Đặt lịch xét nghiệm
                    </Link>

                    <Link
                      to="/dich-vu"
                      className="btn btn-outline-secondary"
                    >
                      Xem dịch vụ khác
                    </Link>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}