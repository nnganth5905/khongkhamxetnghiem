import React, { useState } from 'react';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import {
  checkInAppointment,
  getReceptionAppointments,
} from '../../services/appointmentService';

import { getApiErrorMessage } from '../../services/api';

const normalizeAppointment = (item = {}) => ({
  id:
    item.id ??
    item.idDatLich ??
    item.IDDatLich,

  code:
    item.code ??
    item.maDatLich ??
    item.MaDatLich ??
    '—',

  patientName:
    item.patientName ??
    item.tenKhachHang ??
    item.TenKhachHang ??
    '—',

  phone:
    item.phone ??
    item.soDienThoai ??
    item.SoDienThoai ??
    '—',

  type:
    item.type ??
    item.loaiLich ??
    item.LoaiLich ??
    '—',

  date:
    item.date ??
    item.ngayHen ??
    item.NgayHen ??
    '—',

  time:
    item.time ??
    item.gioHen ??
    item.GioHen ??
    '—',

  doctorName:
    item.doctorName ??
    item.tenBacSi ??
    item.TenBacSi ??
    '—',

  status:
    item.status ??
    item.trangThai ??
    item.TrangThai ??
    'BOOKED',
});

export default function CheckIn() {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState([]);

  const [searching, setSearching] = useState(false);
  const [processingId, setProcessingId] = useState(null);

  const [message, setMessage] = useState({
    type: '',
    text: '',
  });

  const handleSearch = async (e) => {
    e.preventDefault();

    const q = query.trim();

    if (!q) {
      setMessage({
        type: 'warning',
        text: 'Vui lòng nhập mã lịch hẹn, số điện thoại hoặc email.',
      });

      return;
    }

    try {
      setSearching(true);

      const data =
        await getReceptionAppointments({
          q,
        });

      const list = Array.isArray(data)
        ? data
        : data?.content ||
          data?.items ||
          data?.data ||
          [];

      setResults(
        list.map(normalizeAppointment)
      );
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(
          err,
          'Không thể tìm lịch hẹn.'
        ),
      });
    } finally {
      setSearching(false);
    }
  };

  const handleCheckIn = async (appointment) => {
    if (!appointment.id) {
      setMessage({
        type: 'danger',
        text: 'Không xác định được ID lịch hẹn.',
      });

      return;
    }

    try {
      setProcessingId(appointment.id);

      const data =
        await checkInAppointment({
          appointmentId: appointment.id,
        });

      setMessage({
        type: 'success',
        text:
          data?.message ||
          'Check-in thành công.',
      });

      setResults((prev) =>
        prev.map((item) =>
          item.id === appointment.id
            ? {
                ...item,
                status: 'CHECKED_IN',
              }
            : item
        )
      );
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(
          err,
          'Không thể check-in lịch hẹn.'
        ),
      });
    } finally {
      setProcessingId(null);
    }
  };

  return (
    <div>
      <h1 className="dashboard-page-title">
        Check-in lịch hẹn
      </h1>

      <p className="text-secondary mb-4">
        Tìm lịch hẹn của khách hàng và xác nhận khách đã đến cơ sở.
      </p>

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
          <form
            className="row g-3"
            onSubmit={handleSearch}
          >
            <div className="col-lg-9">
              <label className="form-label fw-semibold">
                Thông tin tra cứu
              </label>

              <div className="input-group input-group-lg">
                <span className="input-group-text bg-white">
                  <i className="fa-solid fa-magnifying-glass text-secondary" />
                </span>

                <input
                  type="search"
                  className="form-control"
                  placeholder="Mã lịch hẹn / số điện thoại / email..."
                  value={query}
                  onChange={(e) => setQuery(e.target.value)}
                />
              </div>
            </div>

            <div className="col-lg-3 d-flex align-items-end">
              <button
                type="submit"
                className="btn btn-primary btn-lg w-100"
                disabled={searching}
              >
                {searching
                  ? 'Đang tìm...'
                  : 'Tìm lịch hẹn'}
              </button>
            </div>
          </form>
        </div>
      </div>

      {searching ? (
        <Loading text="Đang tìm lịch hẹn..." />
      ) : (
        <div className="row g-3">
          {results.map((item, index) => {
            const checkedIn =
              String(item.status).toUpperCase() === 'CHECKED_IN';

            return (
              <div
                className="col-12"
                key={item.id ?? index}
              >
                <div className="card border-0 shadow-sm rounded-4">
                  <div className="card-body p-4">
                    <div className="row g-3 align-items-center">
                      <div className="col-lg-2">
                        <div className="small text-secondary">
                          Mã lịch
                        </div>

                        <div className="fw-bold">
                          {item.code}
                        </div>
                      </div>

                      <div className="col-lg-3">
                        <div className="small text-secondary">
                          Khách hàng
                        </div>

                        <div className="fw-bold">
                          {item.patientName}
                        </div>

                        <div className="small text-secondary">
                          {item.phone}
                        </div>
                      </div>

                      <div className="col-lg-2">
                        <div className="small text-secondary">
                          Loại lịch
                        </div>

                        <div>
                          {item.type}
                        </div>
                      </div>

                      <div className="col-lg-2">
                        <div className="small text-secondary">
                          Thời gian
                        </div>

                        <div>
                          {item.date}
                        </div>

                        <div className="fw-semibold">
                          {item.time}
                        </div>
                      </div>

                      <div className="col-lg-3 text-lg-end">
                        {checkedIn ? (
                          <span className="badge bg-success fs-6 px-3 py-2">
                            <i className="fa-solid fa-check me-2" />
                            Đã check-in
                          </span>
                        ) : (
                          <button
                            type="button"
                            className="btn btn-success"
                            disabled={
                              processingId === item.id
                            }
                            onClick={() =>
                              handleCheckIn(item)
                            }
                          >
                            <i className="fa-solid fa-right-to-bracket me-2" />

                            {processingId === item.id
                              ? 'Đang xử lý...'
                              : 'Xác nhận check-in'}
                          </button>
                        )}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            );
          })}

          {!searching &&
            query &&
            results.length === 0 && (
              <div className="col-12">
                <div className="alert alert-light border text-center">
                  Không tìm thấy lịch hẹn phù hợp.
                </div>
              </div>
            )}
        </div>
      )}
    </div>
  );
}