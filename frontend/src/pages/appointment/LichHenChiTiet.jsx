import React, {
  useEffect,
  useState,
} from 'react';

import { Link } from 'react-router-dom';

import {
  getApiErrorMessage,
  getMyAppointments,
} from '../../services/appointmentService';

const formatVND = (amount) => {
  return (
    new Intl.NumberFormat(
      'vi-VN'
    ).format(
      Number(amount || 0)
    ) + ' đ'
  );
};

const formatDate = (dateStr) => {
  if (!dateStr) {
    return '—';
  }

  const cleanDate =
    String(dateStr).substring(
      0,
      10
    );

  const parts =
    cleanDate.split('-');

  if (parts.length === 3) {
    return `${parts[2]}/${parts[1]}/${parts[0]}`;
  }

  const d =
    new Date(dateStr);

  if (
    Number.isNaN(
      d.getTime()
    )
  ) {
    return dateStr;
  }

  const day = String(
    d.getDate()
  ).padStart(2, '0');

  const month = String(
    d.getMonth() + 1
  ).padStart(2, '0');

  const year =
    d.getFullYear();

  return `${day}/${month}/${year}`;
};

const isActiveStatus = (
  status
) => {
  const value = String(
    status ?? ''
  ).toLowerCase();

  return [
    'yes',
    'active',
    'confirmed',
    'da_dat',
    'đã đặt',
  ].includes(value);
};

export default function MyAppointments({
  initialAppointments = null,
}) {
  const [
    appointments,
    setAppointments,
  ] = useState(
    initialAppointments || []
  );

  const [
    loading,
    setLoading,
  ] = useState(
    !initialAppointments
  );

  const [
    error,
    setError,
  ] = useState('');

  useEffect(() => {
    if (
      initialAppointments
    ) {
      return;
    }

    let active = true;

    const loadAppointments =
      async () => {
        try {
          const data =
            await getMyAppointments();

          if (!active) {
            return;
          }

          const items =
            Array.isArray(data)
              ? data
              : data?.appointments ||
                data?.items ||
                [];

          setAppointments(
            items
          );
        } catch (err) {
          if (active) {
            setError(
              getApiErrorMessage(
                err,
                'Không thể tải danh sách lịch hẹn.'
              )
            );
          }
        } finally {
          if (active) {
            setLoading(false);
          }
        }
      };

    loadAppointments();

    return () => {
      active = false;
    };
  }, [
    initialAppointments,
  ]);

  return (
    <div className="bg-light min-vh-100 py-4">
      <div className="container">
        <h3
          className="text-center mb-4 fw-bold"
          style={{
            color: '#0b63e5',
          }}
        >
          Lịch hẹn của tôi
        </h3>

        {loading && (
          <div className="text-center py-4 text-secondary">
            Đang tải lịch hẹn...
          </div>
        )}

        {error && (
          <div className="alert alert-danger">
            {error}
          </div>
        )}

        {!loading &&
          !error &&
          appointments.length ===
            0 && (
            <div className="alert alert-info">
              Chưa có lịch hẹn.
            </div>
          )}

        {!loading &&
          !error &&
          appointments.length >
            0 && (
            <div className="table-responsive">
              <table className="table table-bordered align-middle bg-white shadow-sm rounded">
                <thead className="table-light">
                  <tr>
                    <th>#</th>
                    <th>Ngày</th>
                    <th>Giờ</th>
                    <th>
                      Xét nghiệm
                    </th>
                    <th>
                      Tổng tiền
                    </th>
                    <th>Bác sĩ</th>
                    <th>
                      Người xét
                      nghiệm
                    </th>
                    <th>
                      Trạng thái
                    </th>
                    <th />
                  </tr>
                </thead>

                <tbody>
                  {appointments.map(
                    (
                      item,
                      index
                    ) => {
                      const id =
                        item.AnyID ??
                        item.IDDatLich ??
                        item.id ??
                        item.appointmentId;

                      const rawTests =
                        item.TenXNList ??
                        item.testNames ??
                        [];

                      const testNames =
                        Array.isArray(
                          rawTests
                        )
                          ? rawTests
                          : String(
                              rawTests ||
                                ''
                            )
                              .split(
                                '||'
                              )
                              .filter(
                                Boolean
                              );

                      const date =
                        item.NgayXetNghiem ??
                        item.ngayXetNghiem ??
                        item.date;

                      const time =
                        item.GioXetNghiem ??
                        item.gioXetNghiem ??
                        item.time;

                      const amount =
                        item.TongTien ??
                        item.totalAmount ??
                        item.total;

                      const doctor =
                        item.TenBacSi ??
                        item.doctorName ??
                        item.IDBacSi ??
                        item.doctorId ??
                        '—';

                      const customer =
                        item.TenKhachHang ??
                        item.customerName ??
                        '—';

                      const status =
                        item.Status ??
                        item.status;

                      const activeStatus =
                        isActiveStatus(
                          status
                        );

                      return (
                        <tr
                          key={
                            id ??
                            index
                          }
                        >
                          <td>
                            {index +
                              1}
                          </td>

                          <td>
                            {formatDate(
                              date
                            )}
                          </td>

                          <td>
                            {time
                              ? String(
                                  time
                                ).substring(
                                  0,
                                  5
                                )
                              : '—'}
                          </td>

                          <td
                            style={{
                              minWidth:
                                '320px',
                            }}
                          >
                            {testNames.length ===
                            0 ? (
                              <span className="text-muted">
                                —
                              </span>
                            ) : (
                              testNames.map(
                                (
                                  name,
                                  idx
                                ) => (
                                  <span
                                    key={
                                      idx
                                    }
                                    style={{
                                      background:
                                        '#f1f5ff',

                                      border:
                                        '1px solid #e3ecff',

                                      color:
                                        '#0b3d91',

                                      margin:
                                        '2px 4px 2px 0',

                                      padding:
                                        '0.35rem 0.55rem',

                                      borderRadius:
                                        '999px',

                                      display:
                                        'inline-block',

                                      fontSize:
                                        '0.85rem',
                                    }}
                                  >
                                    {
                                      name
                                    }
                                  </span>
                                )
                              )
                            )}
                          </td>

                          <td>
                            <strong>
                              {formatVND(
                                amount
                              )}
                            </strong>
                          </td>

                          <td>
                            {doctor}
                          </td>

                          <td>
                            {customer}
                          </td>

                          <td>
                            <span
                              className={`badge ${
                                activeStatus
                                  ? 'bg-success'
                                  : 'bg-secondary'
                              }`}
                            >
                              {activeStatus
                                ? 'Hiệu lực'
                                : 'Đã hủy'}
                            </span>
                          </td>

                          <td>
                            <Link
                              className="btn btn-sm btn-primary"
                              to={`/lich-hen/${id}`}
                              style={{
                                backgroundColor:
                                  'var(--primary, #0b63e5)',

                                borderColor:
                                  'var(--primary, #0b63e5)',
                              }}
                            >
                              Chi
                              tiết
                            </Link>
                          </td>
                        </tr>
                      );
                    }
                  )}
                </tbody>
              </table>
            </div>
          )}
      </div>
    </div>
  );
}