import React from 'react';

import {
  Link,
  useLocation,
  useNavigate,
} from 'react-router-dom';

import {
  clearAuthSession,
} from '../services/authService';

const MENUS = {
  ADMIN: [
    {
      label:
        'Tổng quan',
      items: [
        {
          label:
            'Báo cáo thống kê',

          icon:
            'fa-solid fa-chart-pie',

          path:
            '/admin',
        },
      ],
    },

    {
      label:
        'Quản lý',

      items: [
        {
          label:
            'Khách hàng',

          icon:
            'fa-solid fa-users',

          path:
            '/admin/khach-hang',
        },

        {
          label:
            'Nhân viên',

          icon:
            'fa-solid fa-user-gear',

          path:
            '/admin/nhan-vien',
        },

        {
          label:
            'Bác sĩ',

          icon:
            'fa-solid fa-user-doctor',

          path:
            '/admin/bac-si',
        },

        {
          label:
            'Xét nghiệm',

          icon:
            'fa-solid fa-vials',

          path:
            '/admin/xet-nghiem',
        },

        {
          label:
            'Lịch hẹn',

          icon:
            'fa-regular fa-calendar-check',

          path:
            '/admin/lich-hen',
        },

        {
          label:
            'Kết quả',

          icon:
            'fa-solid fa-square-poll-horizontal',

          path:
            '/admin/ket-qua',
        },

        {
          label:
            'Theo dõi',

          icon:
            'fa-solid fa-clock-rotate-left',

          path:
            '/admin/tracking',
        },
      ],
    },
  ],

  DOCTOR: [
    {
      label:
        'Bác sĩ',

      items: [
        {
          label:
            'Tổng quan',

          icon:
            'fa-solid fa-chart-line',

          path:
            '/doctor',
        },

        {
          label:
            'Danh sách chờ',

          icon:
            'fa-solid fa-list-ol',

          path:
            '/doctor/danh-sach-cho',
        },

        {
          label:
            'Khám bệnh',

          icon:
            'fa-solid fa-stethoscope',

          path:
            '/doctor/kham',
        },

        {
          label:
            'Chỉ định xét nghiệm',

          icon:
            'fa-solid fa-file-medical',

          path:
            '/doctor/chi-dinh-xet-nghiem',
        },

        {
          label:
            'Duyệt kết quả',

          icon:
            'fa-solid fa-check-double',

          path:
            '/doctor/duyet-ket-qua',
        },

        {
          label:
            'Lịch làm việc',

          icon:
            'fa-regular fa-calendar-days',

          path:
            '/doctor/lich-lam-viec',
        },
      ],
    },
  ],

  RECEPTIONIST: [
    {
      label:
        'Lễ tân',

      items: [
        {
          label:
            'Tổng quan',

          icon:
            'fa-solid fa-chart-line',

          path:
            '/reception',
        },

        {
          label:
            'Check-in',

          icon:
            'fa-solid fa-right-to-bracket',

          path:
            '/reception/check-in',
        },

        {
          label:
            'Quét QR',

          icon:
            'fa-solid fa-qrcode',

          path:
            '/reception/qr',
        },

        {
          label:
            'Danh sách chờ',

          icon:
            'fa-solid fa-list',

          path:
            '/reception/danh-sach-cho',
        },

        {
          label:
            'Tiếp nhận',

          icon:
            'fa-solid fa-clipboard-user',

          path:
            '/reception/tiep-nhan',
        },
      ],
    },
  ],

  TECHNICIAN: [
    {
      label:
        'Kỹ thuật viên',

      items: [
        {
          label:
            'Tổng quan',

          icon:
            'fa-solid fa-chart-line',

          path:
            '/technician',
        },

        {
          label:
            'Mẫu bệnh phẩm',

          icon:
            'fa-solid fa-vial',

          path:
            '/technician/mau-benh-pham',
        },

        {
          label:
            'Nhận bàn giao',

          icon:
            'fa-solid fa-handshake',

          path:
            '/technician/ban-giao-mau',
        },

        {
          label:
            'Worklist',

          icon:
            'fa-solid fa-list-check',

          path:
            '/technician/worklist',
        },

        {
          label:
            'Nhập kết quả',

          icon:
            'fa-solid fa-file-pen',

          path:
            '/technician/nhap-ket-qua',
        },
      ],
    },
  ],

  CUSTOMER: [
    {
      label:
        'Khách hàng',

      items: [
        {
          label:
            'Tổng quan',

          icon:
            'fa-solid fa-house',

          path:
            '/customer',
        },

        {
          label:
            'Lịch hẹn',

          icon:
            'fa-regular fa-calendar-check',

          path:
            '/lich-hen',
        },

        {
          label:
            'Kết quả xét nghiệm',

          icon:
            'fa-solid fa-file-waveform',

          path:
            '/ket-qua',
        },

        {
          label:
            'Đặt lịch khám',

          icon:
            'fa-solid fa-stethoscope',

          path:
            '/dat-lich-kham',
        },

        {
          label:
            'Đặt lịch xét nghiệm',

          icon:
            'fa-solid fa-vials',

          path:
            '/dat-lich-xet-nghiem',
        },
      ],
    },
  ],
};

const normalizeRole = (
  role
) =>
  String(
    role || 'CUSTOMER'
  ).toUpperCase();

export default function Sidebar({
  role = 'CUSTOMER',
  open = false,
  onClose = null,
}) {
  const location =
    useLocation();

  const navigate =
    useNavigate();

  const normalizedRole =
    normalizeRole(role);

  const menu =
    MENUS[
      normalizedRole
    ] ||
    MENUS.CUSTOMER;

  const isActive =
    (path) => {
      if (
        path ===
        '/admin'
      ) {
        return (
          location.pathname ===
          '/admin'
        );
      }

      if (
        path ===
        '/doctor'
      ) {
        return (
          location.pathname ===
          '/doctor'
        );
      }

      if (
        path ===
        '/customer'
      ) {
        return (
          location.pathname ===
          '/customer'
        );
      }

      if (
        path ===
        '/reception'
      ) {
        return (
          location.pathname ===
          '/reception'
        );
      }

      if (
        path ===
        '/technician'
      ) {
        return (
          location.pathname ===
          '/technician'
        );
      }

      return (
        location.pathname ===
          path ||
        location.pathname.startsWith(
          `${path}/`
        )
      );
    };

  const handleLogout =
    () => {
      clearAuthSession();

      window.dispatchEvent(
        new Event(
          'auth-change'
        )
      );

      navigate(
        '/login',
        {
          replace: true,
        }
      );
    };

  return (
    <aside
      className={`dashboard-sidebar ${
        open
          ? 'is-open'
          : ''
      }`}
    >
      <div className="dashboard-sidebar-brand">
        <img
          src="https://nganngan5905-tech.github.io/LTWEB/Screenshot_2025-09-16_080100-removebg-preview.png"
          alt="Bio Medic"
          className="dashboard-sidebar-logo"
        />

        <div>
          <div
            className="fw-bold"
            style={{
              color:
                'var(--primary)',
            }}
          >
            Bio Medic
          </div>

          <div className="small text-secondary">
            Center
          </div>
        </div>

        {onClose && (
          <button
            type="button"
            className="btn btn-sm ms-auto d-lg-none"
            onClick={
              onClose
            }
          >
            <i className="fa-solid fa-xmark" />
          </button>
        )}
      </div>

      <div className="dashboard-sidebar-menu">
        {menu.map(
          (
            group,
            groupIndex
          ) => (
            <div
              key={
                groupIndex
              }
              className="mb-3"
            >
              <div className="dashboard-sidebar-label">
                {
                  group.label
                }
              </div>

              {group.items.map(
                (item) => (
                  <Link
                    key={
                      item.path
                    }
                    to={
                      item.path
                    }
                    className={`dashboard-sidebar-link ${
                      isActive(
                        item.path
                      )
                        ? 'active'
                        : ''
                    }`}
                    onClick={() =>
                      onClose?.()
                    }
                  >
                    <i
                      className={
                        item.icon
                      }
                    />

                    <span>
                      {
                        item.label
                      }
                    </span>
                  </Link>
                )
              )}
            </div>
          )
        )}
      </div>

      <div className="dashboard-sidebar-footer">
        <Link
          to="/"
          className="dashboard-sidebar-link"
        >
          <i className="fa-solid fa-house" />

          <span>
            Trang chủ
          </span>
        </Link>

        <button
          type="button"
          className="dashboard-sidebar-link border-0 bg-transparent w-100 text-start text-danger"
          onClick={
            handleLogout
          }
        >
          <i className="fa-solid fa-right-from-bracket" />

          <span>
            Đăng xuất
          </span>
        </button>
      </div>
    </aside>
  );
}