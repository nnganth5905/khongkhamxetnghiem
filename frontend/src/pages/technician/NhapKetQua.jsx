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

import {
  getResultEntry,
  saveResultEntry,
  submitResultEntry,
} from '../../services/technicianService';

import {
  getApiErrorMessage,
} from '../../services/api';

const evaluationLabel = (
  evaluation,
) => {
  switch (
    String(
      evaluation ||
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

const evaluationClass = (
  evaluation,
) => {
  switch (
    String(
      evaluation ||
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

export default function NhapKetQua() {
  const {
    worklistId,
  } = useParams();

  const [
    workInfo,
    setWorkInfo,
  ] = useState(null);

  const [
    indicators,
    setIndicators,
  ] = useState([]);

  const [
    generalResult,
    setGeneralResult,
  ] = useState('');

  const [
    notes,
    setNotes,
  ] = useState('');

  const [
    loading,
    setLoading,
  ] = useState(true);

  const [
    saving,
    setSaving,
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

  const loadResultEntry =
    async () => {
      if (!worklistId) {
        setMessage({
          type:
            'danger',

          text:
            'Thiếu mã worklist.',
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
          await getResultEntry(
            worklistId,
          );

        setWorkInfo(
          data,
        );

        const list =
          Array.isArray(
            data?.indicators,
          )
            ? data.indicators
            : [];

        setIndicators(
          list.map(
            (item) => ({
              indicatorId:
                item.indicatorId,

              name:
                item.name,

              unit:
                item.unit ||
                '',

              dataType:
                item.dataType ||
                'number',

              value:
                item.value ??
                '',

              min:
                item.min,

              max:
                item.max,

              reference:
                item.reference ||
                '',

              evaluation:
                item.evaluation ||
                'chua_danh_gia',

              abnormal:
                Boolean(
                  item.abnormal,
                ),

              note:
                item.note ||
                '',
            }),
          ),
        );

        setGeneralResult(
          data?.generalResult ||
          '',
        );

        setNotes(
          data?.notes ||
          '',
        );
      } catch (err) {
        setMessage({
          type:
            'danger',

          text:
            getApiErrorMessage(
              err,
              'Không thể tải phiếu nhập kết quả.',
            ),
        });
      } finally {
        setLoading(
          false,
        );
      }
    };

  useEffect(() => {
    loadResultEntry();
  }, [worklistId]);

  // =====================================================
  // EDIT
  // =====================================================

  const updateIndicator =
    (
      index,
      field,
      value,
    ) => {
      setIndicators(
        (prev) =>
          prev.map(
            (
              item,
              i,
            ) =>
              i === index
                ? {
                    ...item,
                    [field]:
                      value,
                  }
                : item,
          ),
      );
    };

  const buildPayload =
    () => ({
      indicators:
        indicators.map(
          (item) => ({
            indicatorId:
              item.indicatorId,

            value:
              String(
                item.value ??
                '',
              ).trim(),

            abnormal:
              Boolean(
                item.abnormal,
              ),

            note:
              item.note ||
              null,
          }),
        ),

      generalResult:
        generalResult.trim() ||
        null,

      notes:
        notes.trim() ||
        null,
    });

  // =====================================================
  // VALIDATE
  // =====================================================

  const validateBeforeSubmit =
    () => {
      if (
        indicators.length ===
        0
      ) {
        setMessage({
          type:
            'warning',

          text:
            'Xét nghiệm chưa được cấu hình chỉ số.',
        });

        return false;
      }

      const empty =
        indicators.find(
          (item) =>
            !String(
              item.value ??
              '',
            ).trim(),
        );

      if (empty) {
        setMessage({
          type:
            'warning',

          text:
            `Vui lòng nhập kết quả cho "${empty.name}".`,
        });

        return false;
      }

      return true;
    };

  // =====================================================
  // SAVE DRAFT
  // =====================================================

  const handleSaveDraft =
    async () => {
      try {
        setSaving(
          true,
        );

        setMessage({
          type: '',
          text: '',
        });

        const result =
          await saveResultEntry(
            worklistId,
            buildPayload(),
          );

        setMessage({
          type:
            'success',

          text:
            result?.message ||
            'Đã lưu nháp.',
        });

        await loadResultEntry();
      } catch (err) {
        setMessage({
          type:
            'danger',

          text:
            getApiErrorMessage(
              err,
              'Không thể lưu nháp kết quả.',
            ),
        });
      } finally {
        setSaving(
          false,
        );
      }
    };

  // =====================================================
  // SUBMIT
  // =====================================================

  const handleSubmit =
    async () => {
      if (
        !validateBeforeSubmit()
      ) {
        return;
      }

      if (
        !window.confirm(
          'Xác nhận gửi kết quả sang bác sĩ duyệt? Sau khi gửi bạn sẽ không thể chỉnh sửa.',
        )
      ) {
        return;
      }

      try {
        setSaving(
          true,
        );

        setMessage({
          type: '',
          text: '',
        });

        const result =
          await submitResultEntry(
            worklistId,
            buildPayload(),
          );

        setMessage({
          type:
            'success',

          text:
            result?.message ||
            'Đã gửi kết quả sang bác sĩ duyệt.',
        });

        await loadResultEntry();
      } catch (err) {
        setMessage({
          type:
            'danger',

          text:
            getApiErrorMessage(
              err,
              'Không thể gửi duyệt kết quả.',
            ),
        });
      } finally {
        setSaving(
          false,
        );
      }
    };

  if (loading) {
    return (
      <Loading text="Đang tải phiếu nhập kết quả..." />
    );
  }

  const editable =
    workInfo?.editable !==
    false;

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Nhập kết quả xét nghiệm
          </h1>

          <p className="text-secondary mb-0">
            Nhập kết quả theo từng chỉ số của xét nghiệm.
          </p>
        </div>

        <Link
          to="/technician/worklist"
          className="btn btn-outline-secondary"
        >
          <i className="fa-solid fa-arrow-left me-2" />
          Worklist
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

      {!workInfo ? (
        <div className="alert alert-warning">
          Không tìm thấy worklist.
        </div>
      ) : (
        <>
          {/* INFO */}

          <div className="card border-0 shadow-sm rounded-4 mb-4">
            <div className="card-body p-4">
              <div className="row g-3">
                <Info
                  label="Người bệnh"
                  value={
                    workInfo.patientName
                  }
                />

                <Info
                  label="Xét nghiệm"
                  value={
                    workInfo.testName
                  }
                />

                <Info
                  label="Barcode"
                  value={
                    workInfo.specimenCode
                  }
                />

                <Info
                  label="Trạng thái"
                  value={
                    workInfo.resultStatus ||
                    workInfo.worklistStatus
                  }
                />
              </div>
            </div>
          </div>

          {!editable && (
            <div className="alert alert-info">
              Kết quả đã được gửi duyệt hoặc đã được xử lý. Phiếu hiện ở chế độ chỉ đọc.
            </div>
          )}

          {/* INDICATORS */}

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

                      <th style={{
                        minWidth:
                          180,
                      }}>
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

                      <th style={{
                        minWidth:
                          120,
                      }}>
                        Override
                      </th>
                    </tr>
                  </thead>

                  <tbody>
                    {indicators.map(
                      (
                        item,
                        index,
                      ) => (
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

                          <td>
                            <input
                              type={
                                item.dataType ===
                                'number'
                                  ? 'text'
                                  : 'text'
                              }
                              inputMode={
                                item.dataType ===
                                'number'
                                  ? 'decimal'
                                  : undefined
                              }
                              className="form-control"
                              value={
                                item.value
                              }
                              disabled={
                                !editable ||
                                saving
                              }
                              onChange={(e) =>
                                updateIndicator(
                                  index,
                                  'value',
                                  e.target.value,
                                )
                              }
                            />
                          </td>

                          <td>
                            {item.unit ||
                              '—'}
                          </td>

                          <td>
                            {item.reference ||
                              'Chưa cấu hình'}
                          </td>

                          <td>
                            <span
                              className={`badge ${evaluationClass(
                                item.evaluation,
                              )}`}
                            >
                              {evaluationLabel(
                                item.evaluation,
                              )}
                            </span>
                          </td>

                          <td>
                            <div className="form-check">
                              <input
                                type="checkbox"
                                className="form-check-input"
                                checked={
                                  item.abnormal
                                }
                                disabled={
                                  !editable ||
                                  saving
                                }
                                onChange={(e) =>
                                  updateIndicator(
                                    index,
                                    'abnormal',
                                    e.target.checked,
                                  )
                                }
                              />

                              <label className="form-check-label small">
                                Bất thường
                              </label>
                            </div>
                          </td>
                        </tr>
                      ),
                    )}

                    {indicators.length ===
                      0 && (
                      <tr>
                        <td
                          colSpan="6"
                          className="text-center text-secondary py-4"
                        >
                          Chưa cấu hình chỉ số cho xét nghiệm này.
                        </td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          {/* GENERAL */}

          <div className="card border-0 shadow-sm rounded-4">
            <div className="card-body p-4">
              <div className="mb-3">
                <label className="form-label fw-semibold">
                  Kết quả tổng quát
                </label>

                <textarea
                  className="form-control"
                  rows="3"
                  maxLength={500}
                  value={
                    generalResult
                  }
                  disabled={
                    !editable ||
                    saving
                  }
                  onChange={(e) =>
                    setGeneralResult(
                      e.target.value,
                    )
                  }
                />
              </div>

              <div className="mb-4">
                <label className="form-label fw-semibold">
                  Ghi chú kỹ thuật viên
                </label>

                <textarea
                  className="form-control"
                  rows="3"
                  maxLength={500}
                  value={
                    notes
                  }
                  disabled={
                    !editable ||
                    saving
                  }
                  onChange={(e) =>
                    setNotes(
                      e.target.value,
                    )
                  }
                />
              </div>

              {editable && (
                <div className="d-flex flex-wrap gap-2">
                  <button
                    type="button"
                    className="btn btn-outline-primary"
                    disabled={
                      saving
                    }
                    onClick={
                      handleSaveDraft
                    }
                  >
                    <i className="fa-solid fa-floppy-disk me-2" />

                    {saving
                      ? 'Đang lưu...'
                      : 'Lưu nháp'}
                  </button>

                  <button
                    type="button"
                    className="btn btn-success"
                    disabled={
                      saving
                    }
                    onClick={
                      handleSubmit
                    }
                  >
                    <i className="fa-solid fa-paper-plane me-2" />
                    Gửi bác sĩ duyệt
                  </button>
                </div>
              )}
            </div>
          </div>
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