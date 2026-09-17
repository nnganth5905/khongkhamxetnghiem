import React, { useEffect, useState } from 'react';
import {
  Link,
  useNavigate,
  useParams,
} from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import {
  getSpecimen,
  receiveSpecimen,
  rejectSpecimen,
} from '../../services/technicianService';

import { getApiErrorMessage } from '../../services/api';

export default function TiepNhanMau() {
  const { specimenId } = useParams();
  const navigate = useNavigate();

  const [specimen, setSpecimen] = useState(null);

  const [form, setForm] = useState({
    condition: 'GOOD',
    notes: '',
  });

  const [loading, setLoading] = useState(true);
  const [processing, setProcessing] = useState(false);

  const [message, setMessage] = useState({
    type: '',
    text: '',
  });

  useEffect(() => {
    let active = true;

    const loadSpecimen = async () => {
      if (!specimenId) {
        setMessage({
          type: 'danger',
          text: 'Thiếu mã mẫu bệnh phẩm.',
        });

        setLoading(false);
        return;
      }

      try {
        setLoading(true);

        const data = await getSpecimen(specimenId);

        if (active) {
          setSpecimen(data);
        }
      } catch (err) {
        if (active) {
          setMessage({
            type: 'danger',
            text: getApiErrorMessage(
              err,
              'Không thể tải thông tin mẫu bệnh phẩm.'
            ),
          });
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    };

    loadSpecimen();

    return () => {
      active = false;
    };
  }, [specimenId]);

  const setField = (name, value) => {
    setForm((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleReceive = async () => {
    try {
      setProcessing(true);

      await receiveSpecimen(
        specimenId,
        form
      );

      setMessage({
        type: 'success',
        text: 'Tiếp nhận mẫu thành công.',
      });

      setTimeout(() => {
        navigate('/technician/worklist');
      }, 700);
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(
          err,
          'Không thể tiếp nhận mẫu.'
        ),
      });
    } finally {
      setProcessing(false);
    }
  };

  const handleReject = async () => {
    const reason = window.prompt(
      'Nhập lý do từ chối mẫu bệnh phẩm:'
    );

    if (reason === null) {
      return;
    }

    if (!reason.trim()) {
      setMessage({
        type: 'warning',
        text: 'Vui lòng nhập lý do từ chối mẫu.',
      });

      return;
    }

    try {
      setProcessing(true);

      await rejectSpecimen(
        specimenId,
        {
          reason: reason.trim(),
        }
      );

      setMessage({
        type: 'warning',
        text: 'Đã từ chối mẫu bệnh phẩm.',
      });

      setTimeout(() => {
        navigate('/technician/mau-benh-pham');
      }, 700);
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(
          err,
          'Không thể từ chối mẫu.'
        ),
      });
    } finally {
      setProcessing(false);
    }
  };

  if (loading) {
    return (
      <Loading text="Đang tải thông tin mẫu..." />
    );
  }

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Tiếp nhận mẫu bệnh phẩm
          </h1>

          <p className="text-secondary mb-0">
            Kiểm tra tình trạng mẫu trước khi đưa vào worklist.
          </p>
        </div>

        <Link
          to="/technician/mau-benh-pham"
          className="btn btn-outline-secondary"
        >
          <i className="fa-solid fa-arrow-left me-2" />
          Danh sách mẫu
        </Link>
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

      <div className="row g-4">
        <div className="col-lg-5">
          <div className="card border-0 shadow-sm rounded-4 h-100">
            <div className="card-body p-4">
              <h5 className="fw-bold mb-4">
                Thông tin mẫu
              </h5>

              <div className="mb-3">
                <div className="small text-secondary">
                  Mã mẫu
                </div>

                <div className="fw-semibold">
                  {specimen?.code ??
                    specimen?.maMau ??
                    specimen?.MaMau ??
                    specimenId}
                </div>
              </div>

              <div className="mb-3">
                <div className="small text-secondary">
                  Mã vạch
                </div>

                <div className="fw-semibold">
                  {specimen?.barcode ??
                    specimen?.maVach ??
                    specimen?.MaVach ??
                    '—'}
                </div>
              </div>

              <div className="mb-3">
                <div className="small text-secondary">
                  Người bệnh
                </div>

                <div className="fw-semibold">
                  {specimen?.patientName ??
                    specimen?.tenKhachHang ??
                    specimen?.TenKhachHang ??
                    '—'}
                </div>
              </div>

              <div className="mb-3">
                <div className="small text-secondary">
                  Loại mẫu
                </div>

                <div className="fw-semibold">
                  {specimen?.specimenType ??
                    specimen?.loaiMau ??
                    specimen?.LoaiMau ??
                    '—'}
                </div>
              </div>

              <div className="mb-3">
                <div className="small text-secondary">
                  Thời gian lấy mẫu
                </div>

                <div className="fw-semibold">
                  {specimen?.collectedAt ??
                    specimen?.thoiGianLay ??
                    specimen?.ThoiGianLay ??
                    '—'}
                </div>
              </div>

              <div>
                <div className="small text-secondary">
                  Người bàn giao
                </div>

                <div className="fw-semibold">
                  {specimen?.handoverBy ??
                    specimen?.nguoiBanGiao ??
                    '—'}
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className="col-lg-7">
          <div className="card border-0 shadow-sm rounded-4">
            <div className="card-body p-4">
              <h5 className="fw-bold mb-4">
                Đánh giá mẫu
              </h5>

              <div className="mb-3">
                <label className="form-label fw-semibold">
                  Tình trạng mẫu
                </label>

                <select
                  className="form-select"
                  value={form.condition}
                  onChange={(e) =>
                    setField(
                      'condition',
                      e.target.value
                    )
                  }
                >
                  <option value="GOOD">
                    Đạt yêu cầu
                  </option>

                  <option value="WARNING">
                    Có lưu ý nhưng vẫn có thể xử lý
                  </option>
                </select>
              </div>

              <div className="mb-4">
                <label className="form-label fw-semibold">
                  Ghi chú kỹ thuật viên
                </label>

                <textarea
                  className="form-control"
                  rows="5"
                  placeholder="Ví dụ: lượng mẫu đủ, không tan huyết, bao bì nguyên vẹn..."
                  value={form.notes}
                  onChange={(e) =>
                    setField(
                      'notes',
                      e.target.value
                    )
                  }
                />
              </div>

              <div className="alert alert-light border">
                <i className="fa-solid fa-circle-info me-2 text-primary" />
                Chỉ tiếp nhận khi mẫu đáp ứng điều kiện xét nghiệm.
                Nếu mẫu không đạt, chọn <strong>Từ chối mẫu</strong> và
                nhập lý do để hệ thống lưu truy vết.
              </div>

              <div className="d-flex flex-wrap gap-2 mt-4">
                <button
                  type="button"
                  className="btn btn-success"
                  disabled={processing}
                  onClick={handleReceive}
                >
                  <i className="fa-solid fa-check me-2" />

                  {processing
                    ? 'Đang xử lý...'
                    : 'Xác nhận tiếp nhận'}
                </button>

                <button
                  type="button"
                  className="btn btn-outline-danger"
                  disabled={processing}
                  onClick={handleReject}
                >
                  <i className="fa-solid fa-xmark me-2" />
                  Từ chối mẫu
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}