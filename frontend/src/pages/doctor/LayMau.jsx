import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import {
  useNavigate,
  useSearchParams,
} from 'react-router-dom';

import {
  collectSpecimen,
  getPendingSpecimenItems,
} from '../../services/doctorService';

import {
  getApiErrorMessage,
} from '../../services/api';

export default function LayMau() {
  const navigate =
    useNavigate();

  const [
    searchParams,
  ] = useSearchParams();

  const appointmentId =
    searchParams.get('id');

  const [
    pendingItems,
    setPendingItems,
  ] = useState([]);

  const [
    selectedItemId,
    setSelectedItemId,
  ] = useState('');

  const [
    form,
    setForm,
  ] = useState({
    specimenType:
      '',

    barcode:
      '',

    notes:
      '',
  });

  const [
    collected,
    setCollected,
  ] = useState(null);

  const [
    loading,
    setLoading,
  ] = useState(false);

  const [
    fetching,
    setFetching,
  ] = useState(true);

  const [
    message,
    setMessage,
  ] = useState({
    type: '',
    text: '',
  });

  // =====================================================
  // SELECTED
  // =====================================================

  const selectedItem =
    useMemo(
      () =>
        pendingItems.find(
          (item) =>
            String(
              item.testOrderItemId,
            ) ===
            String(
              selectedItemId,
            ),
        ) || null,
      [
        pendingItems,
        selectedItemId,
      ],
    );

  // =====================================================
  // LOAD PENDING
  // =====================================================

  const loadPendingItems =
    async () => {
      if (!appointmentId) {
        setMessage({
          type:
            'danger',

          text:
            'Thiếu mã lịch xét nghiệm.',
        });

        setFetching(
          false,
        );

        return;
      }

      try {
        setFetching(
          true,
        );

        const data =
          await getPendingSpecimenItems(
            appointmentId,
          );

        const list =
          Array.isArray(data)
            ? data
            : [];

        setPendingItems(
          list,
        );

        if (
          list.length > 0
        ) {
          const first =
            list[0];

          setSelectedItemId(
            String(
              first.testOrderItemId,
            ),
          );

          setForm(
            (prev) => ({
              ...prev,

              specimenType:
                first.defaultSpecimenType ||
                'Máu toàn phần',
            }),
          );
        } else {
          setSelectedItemId(
            '',
          );
        }
      } catch (error) {
        setMessage({
          type:
            'danger',

          text:
            getApiErrorMessage(
              error,
              'Không thể tải danh sách xét nghiệm cần lấy mẫu.',
            ),
        });
      } finally {
        setFetching(
          false,
        );
      }
    };

  useEffect(() => {
    loadPendingItems();
  }, [appointmentId]);

  // =====================================================
  // SELECT TEST
  // =====================================================

  const handleTestChange =
    (e) => {
      const id =
        e.target.value;

      setSelectedItemId(
        id,
      );

      const item =
        pendingItems.find(
          (x) =>
            String(
              x.testOrderItemId,
            ) === id,
        );

      setForm(
        (prev) => ({
          ...prev,

          specimenType:
            item?.defaultSpecimenType ||
            'Máu toàn phần',
        }),
      );
    };

  const handleChange =
    (e) => {
      const {
        name,
        value,
      } = e.target;

      setForm(
        (prev) => ({
          ...prev,
          [name]:
            value,
        }),
      );
    };

  // =====================================================
  // COLLECT
  // =====================================================

  const handleSubmit =
    async (e) => {
      e.preventDefault();

      if (!selectedItem) {
        setMessage({
          type:
            'warning',

          text:
            'Vui lòng chọn xét nghiệm cần lấy mẫu.',
        });

        return;
      }

      if (
        !form.specimenType
          .trim()
      ) {
        setMessage({
          type:
            'warning',

          text:
            'Vui lòng chọn loại mẫu bệnh phẩm.',
        });

        return;
      }

      try {
        setLoading(
          true,
        );

        setMessage({
          type: '',
          text: '',
        });

        const data =
          await collectSpecimen({
            appointmentId,

            testOrderItemId:
              Number(
                selectedItem.testOrderItemId,
              ),

            specimenType:
              form.specimenType,

            barcode:
              form.barcode.trim() ||
              null,

            notes:
              form.notes.trim() ||
              null,
          });

        setCollected(
          data,
        );

        setForm(
          (prev) => ({
            ...prev,

            barcode:
              data?.barcode ||
              prev.barcode,
          }),
        );

        setMessage({
          type:
            'success',

          text:
            `Lấy mẫu thành công. Mã mẫu: ${
              data?.id ||
              '—'
            }, Barcode: ${
              data?.barcode ||
              '—'
            }.`,
        });
      } catch (err) {
        setMessage({
          type:
            'danger',

          text:
            getApiErrorMessage(
              err,
              'Không thể lưu mẫu bệnh phẩm.',
            ),
        });
      } finally {
        setLoading(
          false,
        );
      }
    };

  // =====================================================
  // UI
  // =====================================================

  return (
    <div
      className="container py-4"
      style={{
        maxWidth:
          '820px',
      }}
    >
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="h4 fw-bold mb-1">
            Thu thập mẫu bệnh phẩm
          </h2>

          <p className="text-secondary mb-0">
            Ghi nhận đúng mẫu cho từng xét nghiệm trước khi chuyển lab.
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-secondary btn-sm"
          onClick={() =>
            navigate(
              '/doctor/danh-sach-cho',
            )
          }
        >
          Quay lại
        </button>
      </div>

      {message.text && (
        <div
          className={`alert alert-${message.type}`}
        >
          {
            message.text
          }
        </div>
      )}

      {fetching ? (
        <div className="text-center py-5">
          <div className="spinner-border text-primary mb-3" />

          <div className="text-secondary">
            Đang tải chỉ định xét nghiệm...
          </div>
        </div>
      ) : pendingItems.length ===
          0 &&
        !collected ? (
        <div className="alert alert-info">
          Không còn xét nghiệm nào đang chờ lấy mẫu cho lịch này.
        </div>
      ) : (
        <form
          className="card border-0 shadow-sm rounded-4 p-4"
          onSubmit={
            handleSubmit
          }
        >
          <div className="row g-3">
            <div className="col-md-6">
              <label className="form-label fw-semibold">
                Mã lịch xét nghiệm
              </label>

              <input
                type="text"
                className="form-control bg-light"
                value={
                  appointmentId ||
                  ''
                }
                readOnly
              />
            </div>

            <div className="col-md-6">
              <label className="form-label fw-semibold">
                Người bệnh
              </label>

              <input
                type="text"
                className="form-control bg-light"
                value={
                  selectedItem?.patientName ||
                  collected?.patientName ||
                  '—'
                }
                readOnly
              />
            </div>

            <div className="col-12">
              <label className="form-label fw-semibold">
                Xét nghiệm cần lấy mẫu{' '}
                <span className="text-danger">
                  *
                </span>
              </label>

              <select
                className="form-select"
                value={
                  selectedItemId
                }
                disabled={
                  Boolean(
                    collected,
                  )
                }
                onChange={
                  handleTestChange
                }
                required
              >
                <option value="">
                  -- Chọn xét nghiệm --
                </option>

                {pendingItems.map(
                  (item) => (
                    <option
                      key={
                        item.testOrderItemId
                      }
                      value={
                        item.testOrderItemId
                      }
                    >
                      {item.testName}
                      {' - '}
                      {item.testId}
                    </option>
                  ),
                )}
              </select>
            </div>

            <div className="col-md-6">
              <label className="form-label fw-semibold">
                Loại bệnh phẩm{' '}
                <span className="text-danger">
                  *
                </span>
              </label>

              <select
                name="specimenType"
                className="form-select"
                value={
                  form.specimenType
                }
                onChange={
                  handleChange
                }
                disabled={
                  Boolean(
                    collected,
                  )
                }
                required
              >
                <option value="Máu toàn phần">
                  Máu toàn phần (EDTA)
                </option>

                <option value="Huyết thanh">
                  Huyết thanh (Serum)
                </option>

                <option value="Huyết tương">
                  Huyết tương
                </option>

                <option value="Nước tiểu">
                  Nước tiểu
                </option>

                <option value="Dịch ngoáy">
                  Dịch ngoáy
                </option>

                <option value="Khác">
                  Khác
                </option>
              </select>
            </div>

            <div className="col-md-6">
              <label className="form-label fw-semibold">
                Barcode
              </label>

              <input
                type="text"
                name="barcode"
                className="form-control"
                placeholder="Để trống để server tự sinh"
                value={
                  form.barcode
                }
                onChange={
                  handleChange
                }
                disabled={
                  Boolean(
                    collected,
                  )
                }
                maxLength={64}
              />
            </div>

            <div className="col-12">
              <label className="form-label fw-semibold">
                Ghi chú lâm sàng
              </label>

              <textarea
                name="notes"
                rows="3"
                className="form-control"
                value={
                  form.notes
                }
                onChange={
                  handleChange
                }
                disabled={
                  Boolean(
                    collected,
                  )
                }
                maxLength={255}
              />
            </div>
          </div>

          <div className="mt-4 d-flex gap-3">
            {!collected ? (
              <button
                type="submit"
                className="btn btn-primary px-4"
                disabled={
                  loading ||
                  !selectedItemId
                }
              >
                {loading
                  ? 'Đang lưu...'
                  : 'Xác nhận lấy mẫu'}
              </button>
            ) : (
              <button
                type="button"
                className="btn btn-warning px-4"
                onClick={() =>
                  navigate(
                    `/doctor/ban-giao-mau?specimenId=${encodeURIComponent(
                      collected.id,
                    )}`,
                  )
                }
              >
                <i className="fa-solid fa-truck-fast me-2" />
                Bàn giao mẫu
              </button>
            )}
          </div>
        </form>
      )}
    </div>
  );
}