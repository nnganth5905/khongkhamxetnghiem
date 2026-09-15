import React, { useEffect, useState } from 'react';
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

import { getApiErrorMessage } from '../../services/api';

export default function NhapKetQua() {
  const { worklistId } = useParams();

  const [workInfo, setWorkInfo] = useState(null);
  const [indicators, setIndicators] = useState([]);
  const [notes, setNotes] = useState('');

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const [message, setMessage] = useState({
    type: '',
    text: '',
  });

  useEffect(() => {
    let active = true;

    const loadResultEntry = async () => {
      if (!worklistId) {
        setMessage({
          type: 'danger',
          text: 'Thiếu mã worklist.',
        });

        setLoading(false);
        return;
      }

      try {
        setLoading(true);

        const data =
          await getResultEntry(worklistId);

        if (!active) {
          return;
        }

        setWorkInfo(data);

        const list =
          data?.indicators ??
          data?.chiSo ??
          data?.details ??
          [];

        setIndicators(
          list.map((item) => ({
            indicatorId:
              item.indicatorId ??
              item.id ??
              item.idChiSo ??
              item.IDChiSo,

            name:
              item.name ??
              item.tenChiSo ??
              item.TenChiSo ??
              'Chỉ số',

            value:
              item.value ??
              item.giaTri ??
              item.GiaTri ??
              '',

            unit:
              item.unit ??
              item.donVi ??
              item.DonVi ??
              '',

            reference:
              item.reference ??
              item.nguongThamChieu ??
              item.NguongThamChieu ??
              '',

            abnormal: Boolean(
              item.abnormal ??
              item.batThuong ??
              false
            ),
          }))
        );

        setNotes(
          data?.notes ??
          data?.ghiChu ??
          data?.GhiChu ??
          ''
        );
      } catch (err) {
        if (active) {
          setMessage({
            type: 'danger',
            text: getApiErrorMessage(
              err,
              'Không thể tải phiếu nhập kết quả.'
            ),
          });
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    };

    loadResultEntry();

    return () => {
      active = false;
    };
  }, [worklistId]);

  const updateIndicator = (
    index,
    field,
    value
  ) => {
    setIndicators((prev) =>
      prev.map((item, i) =>
        i === index
          ? {
              ...item,
              [field]: value,
            }
          : item
      )
    );
  };

  const buildPayload = () => ({
    indicators: indicators.map((item) => ({
      indicatorId: item.indicatorId,
      value: item.value,
      abnormal: item.abnormal,
    })),
    notes,
  });

  const validateBeforeSubmit = () => {
    if (indicators.length === 0) {
      setMessage({
        type: 'warning',
        text: 'Phiếu xét nghiệm chưa có chỉ số để nhập.',
      });

      return false;
    }

    const emptyIndex = indicators.findIndex(
      (item) => !String(item.value ?? '').trim()
    );

    if (emptyIndex !== -1) {
      setMessage({
        type: 'warning',
        text: `Vui lòng nhập kết quả cho chỉ số "${indicators[emptyIndex].name}".`,
      });

      return false;
    }

    return true;
  };

  const handleSaveDraft = async () => {
    try {
      setSaving(true);

      await saveResultEntry(
        worklistId,
        buildPayload()
      );

      setMessage({
        type: 'success',
        text: 'Đã lưu nháp kết quả.',
      });
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(
          err,
          'Không thể lưu nháp kết quả.'
        ),
      });
    } finally {
      setSaving(false);
    }
  };

  const handleSubmit = async () => {
    if (!validateBeforeSubmit()) {
      return;
    }

    const confirmed = window.confirm(
      'Gửi kết quả sang bác sĩ duyệt? Sau khi gửi, kết quả có thể bị khóa chỉnh sửa tùy quy trình backend.'
    );

    if (!confirmed) {
      return;
    }

    try {
      setSaving(true);

      await submitResultEntry(
        worklistId,
        buildPayload()
      );

      setMessage({
        type: 'success',
        text: 'Đã gửi kết quả sang bác sĩ duyệt.',
      });
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(
          err,
          'Không thể gửi kết quả.'
        ),
      });
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <Loading text="Đang tải phiếu kết quả..." />
    );
  }

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Nhập kết quả xét nghiệm
          </h1>

          <p className="text-secondary mb-0">
            {workInfo?.testName ??
              workInfo?.tenXetNghiem ??
              'Xét nghiệm'}{' '}
            · Worklist #{worklistId}
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

      <div className="card border-0 shadow-sm rounded-4 mb-4">
        <div className="card-body p-4">
          <div className="row g-3">
            <div className="col-md-4">
              <div className="small text-secondary">
                Người bệnh
              </div>

              <div className="fw-semibold">
                {workInfo?.patientName ??
                  workInfo?.tenKhachHang ??
                  '—'}
              </div>
            </div>

            <div className="col-md-4">
              <div className="small text-secondary">
                Mã mẫu
              </div>

              <div className="fw-semibold">
                {workInfo?.specimenCode ??
                  workInfo?.maMau ??
                  '—'}
              </div>
            </div>

            <div className="col-md-4">
              <div className="small text-secondary">
                Xét nghiệm
              </div>

              <div className="fw-semibold">
                {workInfo?.testName ??
                  workInfo?.tenXetNghiem ??
                  '—'}
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body p-4">
          <h5 className="fw-bold mb-4">
            Các chỉ số xét nghiệm
          </h5>

          <div className="table-responsive">
            <table className="table align-middle">
              <thead className="table-light">
                <tr>
                  <th>
                    Chỉ số
                  </th>

                  <th style={{ minWidth: 170 }}>
                    Kết quả
                  </th>

                  <th>
                    Đơn vị
                  </th>

                  <th>
                    Khoảng tham chiếu
                  </th>

                  <th style={{ minWidth: 150 }}>
                    Đánh giá
                  </th>
                </tr>
              </thead>

              <tbody>
                {indicators.length > 0 ? (
                  indicators.map((item, index) => (
                    <tr key={item.indicatorId ?? index}>
                      <td className="fw-semibold">
                        {item.name}
                      </td>

                      <td>
                        <input
                          type="text"
                          className="form-control"
                          value={item.value}
                          onChange={(e) =>
                            updateIndicator(
                              index,
                              'value',
                              e.target.value
                            )
                          }
                        />
                      </td>

                      <td>
                        {item.unit || '—'}
                      </td>

                      <td>
                        {item.reference || '—'}
                      </td>

                      <td>
                        <select
                          className="form-select form-select-sm"
                          value={
                            item.abnormal
                              ? 'ABNORMAL'
                              : 'NORMAL'
                          }
                          onChange={(e) =>
                            updateIndicator(
                              index,
                              'abnormal',
                              e.target.value === 'ABNORMAL'
                            )
                          }
                        >
                          <option value="NORMAL">
                            Bình thường
                          </option>

                          <option value="ABNORMAL">
                            Bất thường
                          </option>
                        </select>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td
                      colSpan="5"
                      className="text-center text-secondary py-4"
                    >
                      Chưa có chỉ số xét nghiệm được cấu hình.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>

          <div className="mt-4">
            <label className="form-label fw-semibold">
              Ghi chú kỹ thuật viên
            </label>

            <textarea
              className="form-control"
              rows="4"
              placeholder="Ghi chú về mẫu, máy xét nghiệm hoặc quá trình thực hiện..."
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
            />
          </div>

          <div className="d-flex flex-wrap gap-2 mt-4">
            <button
              type="button"
              className="btn btn-outline-primary"
              disabled={saving}
              onClick={handleSaveDraft}
            >
              <i className="fa-regular fa-floppy-disk me-2" />
              Lưu nháp
            </button>

            <button
              type="button"
              className="btn btn-primary"
              disabled={
                saving ||
                indicators.length === 0
              }
              onClick={handleSubmit}
            >
              <i className="fa-solid fa-paper-plane me-2" />

              {saving
                ? 'Đang xử lý...'
                : 'Gửi bác sĩ duyệt'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}