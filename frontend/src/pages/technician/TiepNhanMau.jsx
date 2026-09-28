import React, {
  useEffect,
  useState,
} from 'react';

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

import {
  getApiErrorMessage,
} from '../../services/api';

const formatDateTime =
  (value) => {
    if (!value) {
      return '—';
    }

    const date =
      new Date(value);

    return Number.isNaN(
      date.getTime(),
    )
      ? value
      : date.toLocaleString(
          'vi-VN',
        );
  };

export default function TiepNhanMau() {
  const {
    specimenId,
  } = useParams();

  const navigate =
    useNavigate();

  const [
    specimen,
    setSpecimen,
  ] = useState(null);

  const [
    form,
    setForm,
  ] = useState({
    condition:
      'GOOD',

    notes:
      '',
  });

  const [
    loading,
    setLoading,
  ] = useState(true);

  const [
    processing,
    setProcessing,
  ] = useState(false);

  const [
    message,
    setMessage,
  ] = useState({
    type: '',
    text: '',
  });

  // =====================================================
  // LOAD
  // =====================================================

  useEffect(() => {
    let active = true;

    const load =
      async () => {
        if (!specimenId) {
          setMessage({
            type:
              'danger',

            text:
              'Thiếu mã mẫu bệnh phẩm.',
          });

          setLoading(
            false,
          );

          return;
        }

        try {
          setLoading(
            true,
          );

          const data =
            await getSpecimen(
              specimenId,
            );

          if (active) {
            setSpecimen(
              data,
            );
          }
        } catch (err) {
          if (active) {
            setMessage({
              type:
                'danger',

              text:
                getApiErrorMessage(
                  err,
                  'Không thể tải thông tin mẫu.',
                ),
            });
          }
        } finally {
          if (active) {
            setLoading(
              false,
            );
          }
        }
      };

    load();

    return () => {
      active = false;
    };
  }, [specimenId]);

  // =====================================================
  // RECEIVE
  // =====================================================

  const handleReceive =
    async () => {
      try {
        setProcessing(
          true,
        );

        const result =
          await receiveSpecimen(
            specimenId,
            {
              condition:
                form.condition,

              notes:
                form.notes.trim() ||
                null,
            },
          );

        setMessage({
          type:
            'success',

          text:
            result?.message ||
            'Tiếp nhận mẫu thành công.',
        });

        setTimeout(
          () => {
            navigate(
              '/technician/worklist',
            );
          },
          900,
        );
      } catch (err) {
        setMessage({
          type:
            'danger',

          text:
            getApiErrorMessage(
              err,
              'Không thể tiếp nhận mẫu.',
            ),
        });
      } finally {
        setProcessing(
          false,
        );
      }
    };

  // =====================================================
  // REJECT
  // =====================================================

  const handleReject =
    async () => {
      const reason =
        window.prompt(
          'Nhập lý do từ chối mẫu bệnh phẩm:',
        );

      if (reason === null) {
        return;
      }

      if (!reason.trim()) {
        setMessage({
          type:
            'warning',

          text:
            'Lý do từ chối không được để trống.',
        });

        return;
      }

      try {
        setProcessing(
          true,
        );

        const result =
          await rejectSpecimen(
            specimenId,
            {
              reason:
                reason.trim(),
            },
          );

        setMessage({
          type:
            'warning',

          text:
            result?.message ||
            'Đã từ chối mẫu.',
        });

        setTimeout(
          () => {
            navigate(
              '/technician/mau-benh-pham',
            );
          },
          900,
        );
      } catch (err) {
        setMessage({
          type:
            'danger',

          text:
            getApiErrorMessage(
              err,
              'Không thể từ chối mẫu.',
            ),
        });
      } finally {
        setProcessing(
          false,
        );
      }
    };

  if (loading) {
    return (
      <Loading text="Đang tải thông tin mẫu..." />
    );
  }

  const canProcess =
    specimen?.status ===
    'HANDED_OVER';

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Tiếp nhận mẫu bệnh phẩm
          </h1>

          <p className="text-secondary mb-0">
            Kiểm tra mẫu trước khi đưa vào worklist.
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
          type={
            message.type
          }
          message={
            message.text
          }
          onClose={() =>
            setMessage({
              type: '',
              text: '',
            })
          }
        />
      )}

      {!specimen ? (
        <div className="alert alert-danger">
          Không tìm thấy mẫu bệnh phẩm.
        </div>
      ) : (
        <div className="row g-4">
          <div className="col-lg-5">
            <div className="card border-0 shadow-sm rounded-4 h-100">
              <div className="card-body p-4">
                <h5 className="fw-bold mb-4">
                  Thông tin mẫu
                </h5>

                <Info
                  label="Mã mẫu"
                  value={
                    specimen.id
                  }
                />

                <Info
                  label="Barcode"
                  value={
                    specimen.barcode
                  }
                />

                <Info
                  label="Người bệnh"
                  value={
                    specimen.patientName
                  }
                />

                <Info
                  label="Xét nghiệm"
                  value={
                    specimen.testName
                  }
                />

                <Info
                  label="Loại mẫu"
                  value={
                    specimen.specimenType
                  }
                />

                <Info
                  label="Thời gian lấy"
                  value={formatDateTime(
                    specimen.collectedAt,
                  )}
                />

                <Info
                  label="Bác sĩ lấy mẫu"
                  value={
                    specimen.collectorName
                  }
                />

                <Info
                  label="Người bàn giao"
                  value={
                    specimen.handoverBy
                  }
                />

                <Info
                  label="Thời gian bàn giao"
                  value={formatDateTime(
                    specimen.handedOverAt,
                  )}
                />

                <Info
                  label="Trạng thái"
                  value={
                    specimen.status
                  }
                />
              </div>
            </div>
          </div>

          <div className="col-lg-7">
            <div className="card border-0 shadow-sm rounded-4">
              <div className="card-body p-4">
                <h5 className="fw-bold mb-4">
                  Đánh giá mẫu
                </h5>

                {!canProcess && (
                  <div className="alert alert-info">
                    Mẫu hiện ở trạng thái{' '}
                    <strong>
                      {
                        specimen.status
                      }
                    </strong>
                    , không còn chờ tiếp nhận.
                  </div>
                )}

                <div className="mb-3">
                  <label className="form-label fw-semibold">
                    Tình trạng mẫu
                  </label>

                  <select
                    className="form-select"
                    value={
                      form.condition
                    }
                    disabled={
                      !canProcess ||
                      processing
                    }
                    onChange={(e) =>
                      setForm(
                        (prev) => ({
                          ...prev,

                          condition:
                            e.target.value,
                        }),
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
                    maxLength={255}
                    disabled={
                      !canProcess ||
                      processing
                    }
                    value={
                      form.notes
                    }
                    onChange={(e) =>
                      setForm(
                        (prev) => ({
                          ...prev,

                          notes:
                            e.target.value,
                        }),
                      )
                    }
                  />
                </div>

                <div className="alert alert-light border">
                  Nếu mẫu không đạt chất lượng, hãy từ chối mẫu. Hệ thống sẽ tự chuyển chỉ định trở lại trạng thái <strong>chờ lấy mẫu</strong>.
                </div>

                {canProcess && (
                  <div className="d-flex flex-wrap gap-2">
                    <button
                      type="button"
                      className="btn btn-success"
                      disabled={
                        processing
                      }
                      onClick={
                        handleReceive
                      }
                    >
                      <i className="fa-solid fa-check me-2" />

                      {processing
                        ? 'Đang xử lý...'
                        : 'Xác nhận tiếp nhận'}
                    </button>

                    <button
                      type="button"
                      className="btn btn-outline-danger"
                      disabled={
                        processing
                      }
                      onClick={
                        handleReject
                      }
                    >
                      <i className="fa-solid fa-xmark me-2" />
                      Từ chối mẫu
                    </button>
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

function Info({
  label,
  value,
}) {
  return (
    <div className="mb-3">
      <div className="small text-secondary">
        {label}
      </div>

      <div className="fw-semibold">
        {value ||
          '—'}
      </div>
    </div>
  );
}