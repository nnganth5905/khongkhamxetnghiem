import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import { getDashboardStats } from '../../services/adminService';
import { getApiErrorMessage } from '../../services/api';

const FALLBACK = {
  totalCustomers: 0,
  totalEmployees: 0,
  totalDoctors: 0,
  totalTechnicians: 0,
  totalTestOrders: 0,
  totalResults: 0,
  todayAppointments: 0,
  completedToday: 0,
  revenue: 0,
  statusBreakdown: [],
  topTests: [],
};

const formatVnd = (value) => {
  const number =
    Number(value || 0);

  return `${number.toLocaleString(
    'vi-VN'
  )} đ`;
};

export default function BaoCaoThongKe() {
  const [stats, setStats] =
    useState(FALLBACK);

  const [range, setRange] =
    useState('MONTH');

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState('');

  const loadStats = async () => {
    try {
      setLoading(true);
      setError('');

      const data =
        await getDashboardStats(
          {
            range,
          }
        );

      setStats({
        ...FALLBACK,
        ...(data || {}),
      });
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          'Không thể tải báo cáo thống kê.'
        )
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadStats();
  }, [range]);

  const cards = useMemo(
    () => [
      {
        title:
          'Tổng khách hàng',
        value:
          stats.totalCustomers,
        icon:
          'fa-solid fa-users',
      },
      {
        title:
          'Tổng nhân viên',
        value:
          stats.totalEmployees,
        icon:
          'fa-solid fa-users-gear',
      },
      {
        title:
          'Bác sĩ',
        value:
          stats.totalDoctors,
        icon:
          'fa-solid fa-user-doctor',
      },
      {
        title:
          'Kỹ thuật viên',
        value:
          stats.totalTechnicians,
        icon:
          'fa-solid fa-microscope',
      },
      {
        title:
          'Phiếu xét nghiệm',
        value:
          stats.totalTestOrders,
        icon:
          'fa-solid fa-flask-vial',
      },
      {
        title:
          'Kết quả',
        value:
          stats.totalResults,
        icon:
          'fa-solid fa-square-poll-horizontal',
      },
      {
        title:
          'Lịch hôm nay',
        value:
          stats.todayAppointments,
        icon:
          'fa-regular fa-calendar-check',
      },
      {
        title:
          'Hoàn tất hôm nay',
        value:
          stats.completedToday,
        icon:
          'fa-solid fa-circle-check',
      },
    ],
    [stats]
  );

  const maxStatus = Math.max(
    1,
    ...(
      stats.statusBreakdown ??
      []
    ).map(
      (item) =>
        Number(
          item.count ??
            item.value ??
            0
        )
    )
  );

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Báo cáo thống kê
          </h1>

          <p className="text-secondary mb-0">
            Tổng quan hoạt động khám và xét nghiệm của Bio Medic Center.
          </p>
        </div>

        <div className="d-flex gap-2">
          <select
            className="form-select"
            value={range}
            onChange={(e) =>
              setRange(
                e.target.value
              )
            }
          >
            <option value="TODAY">
              Hôm nay
            </option>

            <option value="WEEK">
              7 ngày
            </option>

            <option value="MONTH">
              30 ngày
            </option>

            <option value="YEAR">
              Năm nay
            </option>
          </select>

          <button
            type="button"
            className="btn btn-outline-primary"
            onClick={loadStats}
            disabled={loading}
          >
            <i className="fa-solid fa-rotate" />
          </button>
        </div>
      </div>

      {error && (
        <Notification
          type="danger"
          message={error}
          onClose={() =>
            setError('')
          }
        />
      )}

      {loading ? (
        <Loading text="Đang tải báo cáo..." />
      ) : (
        <>
          <div className="row g-4 mb-4">
            {cards.map(
              (card) => (
                <div
                  className="col-md-6 col-xl-3"
                  key={
                    card.title
                  }
                >
                  <div className="card border-0 shadow-sm rounded-4 h-100">
                    <div className="card-body p-4">
                      <div className="d-flex justify-content-between align-items-start">
                        <div>
                          <div className="text-secondary small">
                            {
                              card.title
                            }
                          </div>

                          <div className="fs-2 fw-bold mt-2">
                            {Number(
                              card.value ||
                                0
                            ).toLocaleString(
                              'vi-VN'
                            )}
                          </div>
                        </div>

                        <div
                          className="d-flex align-items-center justify-content-center rounded-3"
                          style={{
                            width: 48,
                            height: 48,
                            background:
                              '#eaf2ff',
                            color:
                              'var(--primary)',
                          }}
                        >
                          <i
                            className={
                              card.icon
                            }
                          />
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              )
            )}
          </div>

          <div className="row g-4 mb-4">
            <div className="col-lg-4">
              <div
                className="card border-0 shadow-sm rounded-4 h-100"
                style={{
                  background:
                    'var(--primary)',
                  color:
                    '#fff',
                }}
              >
                <div className="card-body p-4">
                  <div className="small opacity-75">
                    Doanh thu trong khoảng chọn
                  </div>

                  <div className="display-6 fw-bold mt-2">
                    {formatVnd(
                      stats.revenue
                    )}
                  </div>

                  <div className="mt-4 small opacity-75">
                    <i className="fa-solid fa-chart-line me-2" />
                    Số liệu lấy từ hóa đơn đã ghi nhận trong hệ thống.
                  </div>
                </div>
              </div>
            </div>

            <div className="col-lg-8">
              <div className="card border-0 shadow-sm rounded-4 h-100">
                <div className="card-body p-4">
                  <h5 className="fw-bold mb-4">
                    Trạng thái quy trình
                  </h5>

                  {(stats.statusBreakdown ??
                    []).length >
                  0 ? (
                    <div className="d-flex flex-column gap-3">
                      {(
                        stats.statusBreakdown ??
                        []
                      ).map(
                        (
                          item,
                          index
                        ) => {
                          const value =
                            Number(
                              item.count ??
                                item.value ??
                                0
                            );

                          const percent =
                            Math.max(
                              3,
                              Math.round(
                                (value /
                                  maxStatus) *
                                  100
                              )
                            );

                          return (
                            <div
                              key={
                                item.status ??
                                item.label ??
                                index
                              }
                            >
                              <div className="d-flex justify-content-between small mb-1">
                                <span>
                                  {item.label ??
                                    item.status ??
                                    'Khác'}
                                </span>

                                <strong>
                                  {value}
                                </strong>
                              </div>

                              <div
                                className="progress"
                                style={{
                                  height:
                                    9,
                                }}
                              >
                                <div
                                  className="progress-bar"
                                  style={{
                                    width: `${percent}%`,
                                    background:
                                      'var(--primary)',
                                  }}
                                />
                              </div>
                            </div>
                          );
                        }
                      )}
                    </div>
                  ) : (
                    <div className="text-secondary">
                      Chưa có dữ liệu trạng thái.
                    </div>
                  )}
                </div>
              </div>
            </div>
          </div>

          <div className="card border-0 shadow-sm rounded-4">
            <div className="card-body p-4">
              <h5 className="fw-bold mb-4">
                Xét nghiệm được sử dụng nhiều
              </h5>

              <div className="table-responsive">
                <table className="table align-middle">
                  <thead className="table-light">
                    <tr>
                      <th>
                        #
                      </th>

                      <th>
                        Xét nghiệm
                      </th>

                      <th>
                        Số lượt
                      </th>

                      <th>
                        Doanh thu
                      </th>
                    </tr>
                  </thead>

                  <tbody>
                    {(stats.topTests ??
                      []).length >
                    0 ? (
                      (
                        stats.topTests ??
                        []
                      ).map(
                        (
                          item,
                          index
                        ) => (
                          <tr
                            key={
                              item.id ??
                              item.testName ??
                              index
                            }
                          >
                            <td>
                              {index +
                                1}
                            </td>

                            <td className="fw-semibold">
                              {item.testName ??
                                item.tenXetNghiem ??
                                '—'}
                            </td>

                            <td>
                              {Number(
                                item.count ??
                                  item.soLuot ??
                                  0
                              ).toLocaleString(
                                'vi-VN'
                              )}
                            </td>

                            <td>
                              {formatVnd(
                                item.revenue ??
                                  item.doanhThu ??
                                  0
                              )}
                            </td>
                          </tr>
                        )
                      )
                    ) : (
                      <tr>
                        <td
                          colSpan="4"
                          className="text-center text-secondary py-4"
                        >
                          Chưa có dữ liệu thống kê xét nghiệm.
                        </td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
}