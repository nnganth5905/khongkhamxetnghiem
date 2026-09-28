import React, {
  useEffect,
  useState,
} from 'react';

import {
  useNavigate,
  useParams,
} from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import {
  approveDoctorResult,
  getDoctorResultDetail,
} from '../../services/doctorService';

import {
  getApiErrorMessage,
} from '../../services/api';

const evaluationText =
  (value) => {
    switch (
      String(
        value ||
        '',
      ).toLowerCase()
    ) {
      case 'binh_thuong':
        return 'Bình thường';

      case 'thap':
        return 'Thấp';

      case 'cao':
        return 'Cao';

      case 'bat_thuong':
        return 'Bất thường';

      default:
        return 'Chưa đánh giá';
    }
  };

const evaluationClass =
  (value) => {
    switch (
      String(
        value ||
        '',
      ).toLowerCase()
    ) {
      case 'binh_thuong':
        return 'bg-success-subtle text-success';

      case 'thap':
      case 'cao':
      case 'bat_thuong':
        return 'bg-danger-subtle text-danger';

      default:
        return 'bg-secondary-subtle text-secondary';
    }
  };

export default function DocKetQua() {
  const {
    id,
  } = useParams();

  const navigate =
    useNavigate();

  const [
    data,
    setData,
  ] = useState(null);

  const [
    conclusion,
    setConclusion,
  ] = useState('');

  const [
    advice,
    setAdvice,
  ] = useState('');

  const [
    loading,
    setLoading,
  ] = useState(true);

  const [
    submitting,
    setSubmitting,
  ] = useState(false);

  const [
    message,
    setMessage,
  ] = useState({
    type: '',
    text: '',
  });

  const load =
    async () => {
      if (!id) {
        setMessage({
          type:
            'danger',

          text:
            'Thiếu mã kết quả.',
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

        const result =
          await getDoctorResultDetail(
            id,
          );

        setData(
          result,
        );

        setConclusion(
          result?.doctorConclusion ||
          '',
        );
      } catch (err) {
        setMessage({
          type:
            'danger',

          text:
            getApiErrorMessage(
              err,
              'Không thể tải kết quả xét nghiệm.',
            ),
        });
      } finally {
        setLoading(
          false,
        );
      }
    };

  useEffect(() => {
    load();
  }, [id]);

  const handleApprove =
    async (e) => {
      e.preventDefault();

      if (
        !conclusion.trim()
      ) {
        setMessage({
          type:
            'warning',

          text:
            'Vui lòng nhập kết luận của bác sĩ.',
        });

        return;
      }

      if (
        !window.confirm(
          'Xác nhận duyệt kết quả xét nghiệm này?',
        )
      ) {
        return;
      }

      try {
        setSubmitting(
          true,
        );

        const finalConclusion =
          advice.trim()
            ? `${conclusion.trim()}\nLời khuyên: ${advice.trim()}`
            : conclusion.trim();

        const result =
          await approveDoctorResult(
            id,
            finalConclusion,
          );

        setMessage({
          type:
            'success',

          text:
            result?.message ||
            'Duyệt kết quả thành công.',
        });

        setTimeout(
          () =>
            navigate(
              '/doctor/duyet-ket-qua',
            ),
          900,
        );
      } catch (err) {
        setMessage({
          type:
            'danger',

          text:
            getApiErrorMessage(
              err,
              'Không thể duyệt kết quả.',
            ),
        });
      } finally {
        setSubmitting(
          false,
        );
      }
    };

  if (loading) {
    return (
      <Loading text="Đang tải kết quả..." />
    );
  }

  const canApprove =
    data?.status ===
    'PENDING_APPROVAL';

  return (
    <div
      className="container py-4"
      style={{
        maxWidth:
          980,
      }}
    >
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="h4 fw-bold mb-1">
            Đọc & duyệt kết quả
          </h2>

          <p className="text-secondary mb-0">
            Kiểm tra chỉ số xét nghiệm và đưa ra kết luận chuyên môn.
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-secondary"
          onClick={() =>
            navigate(
              '/doctor/duyet-ket-qua',
            )
          }
        >
          <i className="fa-solid fa-arrow-left me-2" />
          Quay lại
        </button>
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

      {!data ? (
        <div className="alert alert-warning">
          Không tìm thấy kết quả.
        </div>
      ) : (
        <>
          <div className="card border-0 shadow-sm rounded-4 mb-4">
            <div className="card-body p-4">
              <h5 className="fw-bold text-primary mb-4">
                {
                  data.testName
                }
              </h5>

              <div className="row g-3">
                <Info
                  label="Người bệnh"
                  value={
                    data.patientName
                  }
                />

                <Info
                  label="Barcode"
                  value={
                    data.specimenCode
                  }
                />

                <Info
                  label="Kỹ thuật viên"
                  value={
                    data.technicianName
                  }
                />

                <Info
                  label="Trạng thái"
                  value={
                    data.status
                  }
                />
              </div>

              {data.technicianNotes && (
                <div className="alert alert-light border mt-4 mb-0">
                  <strong>
                    Ghi chú KTV:
                  </strong>{' '}
                  {
                    data.technicianNotes
                  }
                </div>
              )}
            </div>
          </div>

          <div className="card border-0 shadow-sm rounded-4 mb-4">
            <div className="card-body p-4">
              <h5 className="fw-bold mb-4">
                Kết quả chỉ số
              </h5>

              <div className="table-responsive">
                <table className="table table-hover align-middle">
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
                        Tham chiếu
                      </th>

                      <th>
                        Đánh giá
                      </th>
                    </tr>
                  </thead>

                  <tbody>
                    {(data.indicators ||
                      []).map(
                      (item) => (
                        <tr
                          key={
                            item.indicatorId
                          }
                        >
                          <td className="fw-semibold">
                            {
                              item.name
                            }
                          </td>

                          <td
                            className={
                              item.abnormal
                                ? 'text-danger fw-bold'
                                : 'fw-semibold'
                            }
                          >
                            {
                              item.value
                            }
                          </td>

                          <td>
                            {item.unit ||
                              '—'}
                          </td>

                          <td>
                            {item.reference ||
                              '—'}
                          </td>

                          <td>
                            <span
                              className={`badge ${evaluationClass(
                                item.evaluation,
                              )}`}
                            >
                              {evaluationText(
                                item.evaluation,
                              )}
                            </span>
                          </td>
                        </tr>
                      ),
                    )}
                  </tbody>
                </table>
              </div>

              {data.generalResult && (
                <div className="mt-3">
                  <strong>
                    Kết quả tổng quát:
                  </strong>

                  <div className="mt-2">
                    {
                      data.generalResult
                    }
                  </div>
                </div>
              )}
            </div>
          </div>

          <form
            className="card border-0 shadow-sm rounded-4"
            onSubmit={
              handleApprove
            }
          >
            <div className="card-body p-4">
              <h5 className="fw-bold mb-4">
                Kết luận bác sĩ
              </h5>

              <div className="mb-3">
                <label className="form-label fw-semibold">
                  Kết luận{' '}
                  <span className="text-danger">
                    *
                  </span>
                </label>

                <textarea
                  className="form-control"
                  rows="4"
                  value={
                    conclusion
                  }
                  disabled={
                    !canApprove ||
                    submitting
                  }
                  onChange={(e) =>
                    setConclusion(
                      e.target.value,
                    )
                  }
                />
              </div>

              <div className="mb-4">
                <label className="form-label fw-semibold">
                  Lời khuyên
                </label>

                <textarea
                  className="form-control"
                  rows="3"
                  value={
                    advice
                  }
                  disabled={
                    !canApprove ||
                    submitting
                  }
                  onChange={(e) =>
                    setAdvice(
                      e.target.value,
                    )
                  }
                />
              </div>

              {canApprove ? (
                <button
                  type="submit"
                  className="btn btn-success px-4"
                  disabled={
                    submitting
                  }
                >
                  <i className="fa-solid fa-circle-check me-2" />

                  {submitting
                    ? 'Đang duyệt...'
                    : 'Duyệt kết quả'}
                </button>
              ) : (
                <div className="alert alert-success mb-0">
                  Kết quả đã được xử lý.
                </div>
              )}
            </div>
          </form>
        </>
      )}
    </div>
  );
}

function Info({
  label,
  value,
}) {
  return (
    <div className="col-md-3">
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