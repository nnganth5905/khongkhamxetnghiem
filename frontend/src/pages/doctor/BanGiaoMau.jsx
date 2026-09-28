import React, {
  useEffect,
  useState,
} from 'react';

import {
  useNavigate,
  useSearchParams,
} from 'react-router-dom';

import {
  getTechnicians,
  handoverSpecimen,
} from '../../services/doctorService';

import {
  getApiErrorMessage,
} from '../../services/api';

export default function BanGiaoMau() {
  const navigate =
    useNavigate();

  const [
    searchParams,
  ] = useSearchParams();

  const specimenId =
    searchParams.get(
      'specimenId',
    );

  const [
    technicians,
    setTechnicians,
  ] = useState([]);

  const [
    form,
    setForm,
  ] = useState({
    receiverId:
      '',

    note:
      '',
  });

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
  // LOAD KTV
  // =====================================================

  useEffect(() => {
    let active = true;

    const loadTechnicians =
      async () => {
        try {
          setFetching(
            true,
          );

          const data =
            await getTechnicians();

          if (!active) {
            return;
          }

          setTechnicians(
            Array.isArray(data)
              ? data
              : [],
          );
        } catch (error) {
          if (active) {
            setMessage({
              type:
                'danger',

              text:
                getApiErrorMessage(
                  error,
                  'Không thể tải danh sách kỹ thuật viên.',
                ),
            });
          }
        } finally {
          if (active) {
            setFetching(
              false,
            );
          }
        }
      };

    loadTechnicians();

    return () => {
      active = false;
    };
  }, []);

  // =====================================================
  // SUBMIT
  // =====================================================

  const handleSubmit =
    async (e) => {
      e.preventDefault();

      if (!specimenId) {
        setMessage({
          type:
            'danger',

          text:
            'Thiếu mã mẫu bệnh phẩm.',
        });

        return;
      }

      if (!form.receiverId) {
        setMessage({
          type:
            'warning',

          text:
            'Vui lòng chọn kỹ thuật viên tiếp nhận.',
        });

        return;
      }

      try {
        setLoading(
          true,
        );

        const data =
          await handoverSpecimen(
            specimenId,
            {
              receiverId:
                form.receiverId,

              note:
                form.note.trim() ||
                null,
            },
          );

        setMessage({
          type:
            'success',

          text:
            data?.message ||
            'Bàn giao mẫu thành công.',
        });

        setTimeout(
          () => {
            navigate(
              '/doctor/danh-sach-cho',
            );
          },
          1200,
        );
      } catch (error) {
        setMessage({
          type:
            'danger',

          text:
            getApiErrorMessage(
              error,
              'Không thể bàn giao mẫu.',
            ),
        });
      } finally {
        setLoading(
          false,
        );
      }
    };

  return (
    <div
      className="container py-4"
      style={{
        maxWidth:
          '720px',
      }}
    >
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="h4 fw-bold mb-1">
            Bàn giao mẫu bệnh phẩm
          </h2>

          <p className="text-secondary mb-0">
            Chuyển mẫu đến đúng kỹ thuật viên phụ trách.
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-secondary btn-sm"
          onClick={() =>
            navigate(-1)
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

      <form
        className="card border-0 shadow-sm rounded-4 p-4"
        onSubmit={
          handleSubmit
        }
      >
        <div className="mb-3">
          <label className="form-label fw-semibold">
            Mã mẫu bệnh phẩm
          </label>

          <input
            type="text"
            className="form-control bg-light"
            value={
              specimenId ||
              ''
            }
            readOnly
          />
        </div>

        <div className="mb-3">
          <label className="form-label fw-semibold">
            Kỹ thuật viên tiếp nhận{' '}
            <span className="text-danger">
              *
            </span>
          </label>

          <select
            className="form-select"
            value={
              form.receiverId
            }
            onChange={(e) =>
              setForm(
                (prev) => ({
                  ...prev,

                  receiverId:
                    e.target.value,
                }),
              )
            }
            required
            disabled={
              fetching
            }
          >
            <option value="">
              -- Chọn kỹ thuật viên --
            </option>

            {technicians.map(
              (ktv) => (
                <option
                  key={
                    ktv.id
                  }
                  value={
                    ktv.id
                  }
                >
                  {ktv.name}
                  {' ('}
                  {ktv.id}
                  {')'}
                </option>
              ),
            )}
          </select>
        </div>

        <div className="mb-3">
          <label className="form-label fw-semibold">
            Ghi chú bàn giao
          </label>

          <textarea
            rows="3"
            className="form-control"
            value={
              form.note
            }
            maxLength={255}
            onChange={(e) =>
              setForm(
                (prev) => ({
                  ...prev,

                  note:
                    e.target.value,
                }),
              )
            }
          />
        </div>

        <button
          type="submit"
          className="btn btn-primary px-4"
          disabled={
            loading ||
            fetching ||
            !specimenId
          }
        >
          {loading
            ? 'Đang bàn giao...'
            : 'Xác nhận bàn giao'}
        </button>
      </form>
    </div>
  );
}