import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import {
  getNotifications,
  markAllNotificationsRead,
  markNotificationRead,
} from '../../services/notificationService';

import { getApiErrorMessage } from '../../services/api';

const normalizeNotification = (item = {}) => ({
  id:
    item.id ??
    item.idThongBao ??
    item.IDThongBao,

  title:
    item.title ??
    item.tieuDe ??
    item.TieuDe ??
    'Thông báo',

  content:
    item.content ??
    item.noiDung ??
    item.NoiDung ??
    '',

  createdAt:
    item.createdAt ??
    item.ngayTao ??
    item.NgayTao ??
    '—',

  type:
    item.type ??
    item.loai ??
    item.Loai ??
    'INFO',

  read: Boolean(
    item.read ??
    item.daDoc ??
    item.DaDoc ??
    false
  ),
});

const iconByType = (type) => {
  switch (String(type || '').toUpperCase()) {
    case 'SUCCESS':
      return 'fa-solid fa-circle-check text-success';

    case 'WARNING':
      return 'fa-solid fa-triangle-exclamation text-warning';

    case 'ERROR':
    case 'DANGER':
      return 'fa-solid fa-circle-exclamation text-danger';

    case 'APPOINTMENT':
      return 'fa-regular fa-calendar-check text-primary';

    case 'RESULT':
      return 'fa-solid fa-square-poll-horizontal text-primary';

    default:
      return 'fa-regular fa-bell text-primary';
  }
};

export default function ThongBao() {
  const [items, setItems] = useState([]);
  const [filter, setFilter] = useState('ALL');

  const [loading, setLoading] = useState(true);

  const [message, setMessage] = useState({
    type: '',
    text: '',
  });

  const loadNotifications = async () => {
    try {
      setLoading(true);

      const data =
        await getNotifications();

      const list = Array.isArray(data)
        ? data
        : data?.content ||
          data?.items ||
          data?.data ||
          [];

      setItems(
        list.map(
          normalizeNotification
        )
      );
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(
          err,
          'Không thể tải thông báo.'
        ),
      });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadNotifications();
  }, []);

  const unreadCount =
    useMemo(
      () =>
        items.filter(
          (item) => !item.read
        ).length,
      [items]
    );

  const visibleItems =
    useMemo(() => {
      if (filter === 'UNREAD') {
        return items.filter(
          (item) => !item.read
        );
      }

      if (filter === 'READ') {
        return items.filter(
          (item) => item.read
        );
      }

      return items;
    }, [items, filter]);

  const markOne = async (id) => {
    const target = items.find(
      (item) => item.id === id
    );

    if (!target || target.read) {
      return;
    }

    try {
      await markNotificationRead(id);

      setItems((prev) =>
        prev.map((item) =>
          item.id === id
            ? {
                ...item,
                read: true,
              }
            : item
        )
      );
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(
          err,
          'Không thể đánh dấu thông báo đã đọc.'
        ),
      });
    }
  };

  const markAll = async () => {
    if (unreadCount === 0) {
      return;
    }

    try {
      await markAllNotificationsRead();

      setItems((prev) =>
        prev.map((item) => ({
          ...item,
          read: true,
        }))
      );

      setMessage({
        type: 'success',
        text: 'Đã đánh dấu tất cả thông báo là đã đọc.',
      });
    } catch (err) {
      setMessage({
        type: 'danger',
        text: getApiErrorMessage(
          err,
          'Không thể cập nhật toàn bộ thông báo.'
        ),
      });
    }
  };

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
        <div>
          <h1 className="dashboard-page-title mb-1">
            Thông báo
          </h1>

          <p className="text-secondary mb-0">
            Bạn có{' '}
            <strong>
              {unreadCount}
            </strong>{' '}
            thông báo chưa đọc.
          </p>
        </div>

        <button
          type="button"
          className="btn btn-outline-primary"
          onClick={markAll}
          disabled={unreadCount === 0}
        >
          <i className="fa-solid fa-check-double me-2" />
          Đánh dấu tất cả đã đọc
        </button>
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

      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body p-4">
          <div className="d-flex flex-wrap gap-2 mb-4">
            <button
              type="button"
              className={`btn ${
                filter === 'ALL'
                  ? 'btn-primary'
                  : 'btn-outline-primary'
              }`}
              onClick={() => setFilter('ALL')}
            >
              Tất cả
            </button>

            <button
              type="button"
              className={`btn ${
                filter === 'UNREAD'
                  ? 'btn-primary'
                  : 'btn-outline-primary'
              }`}
              onClick={() => setFilter('UNREAD')}
            >
              Chưa đọc ({unreadCount})
            </button>

            <button
              type="button"
              className={`btn ${
                filter === 'READ'
                  ? 'btn-primary'
                  : 'btn-outline-primary'
              }`}
              onClick={() => setFilter('READ')}
            >
              Đã đọc
            </button>
          </div>

          {loading ? (
            <Loading text="Đang tải thông báo..." />
          ) : (
            <div className="d-flex flex-column gap-3">
              {visibleItems.map((item, index) => (
                <button
                  key={item.id ?? index}
                  type="button"
                  className={`card text-start border-0 rounded-4 ${
                    item.read
                      ? 'bg-white'
                      : 'bg-primary-subtle'
                  }`}
                  style={{
                    boxShadow:
                      '0 3px 14px rgba(15, 23, 42, 0.06)',
                  }}
                  onClick={() =>
                    markOne(item.id)
                  }
                >
                  <div className="card-body p-4 w-100">
                    <div className="d-flex gap-3">
                      <div
                        className="d-flex align-items-center justify-content-center rounded-circle flex-shrink-0"
                        style={{
                          width: 46,
                          height: 46,
                          background: '#fff',
                        }}
                      >
                        <i
                          className={`${iconByType(
                            item.type
                          )} fs-5`}
                        />
                      </div>

                      <div className="flex-grow-1">
                        <div className="d-flex justify-content-between gap-3">
                          <h6 className="fw-bold mb-1">
                            {item.title}
                          </h6>

                          {!item.read && (
                            <span className="badge bg-primary">
                              Mới
                            </span>
                          )}
                        </div>

                        <div
                          className="text-secondary"
                          style={{
                            whiteSpace:
                              'pre-line',
                          }}
                        >
                          {item.content}
                        </div>

                        <div className="small text-muted mt-2">
                          {item.createdAt}
                        </div>
                      </div>
                    </div>
                  </div>
                </button>
              ))}

              {visibleItems.length === 0 && (
                <div className="alert alert-light border text-center mb-0">
                  Không có thông báo trong nhóm này.
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}