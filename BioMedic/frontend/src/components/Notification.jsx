import React, {
  useEffect,
} from 'react';

export default function Notification({
  type = 'info',
  message = '',
  onClose = null,
  autoClose = 0,
  fixed = false,
}) {
  useEffect(() => {
    if (
      !autoClose ||
      !onClose
    ) {
      return undefined;
    }

    const timer =
      setTimeout(
        () => {
          onClose();
        },
        autoClose
      );

    return () =>
      clearTimeout(timer);
  }, [
    autoClose,
    onClose,
  ]);

  if (!message) {
    return null;
  }

  return (
    <div
      className={`alert alert-${type} app-notification ${
        fixed
          ? 'app-notification-fixed shadow'
          : ''
      }`}
      role="alert"
    >
      <div className="d-flex align-items-start justify-content-between gap-3">
        <div>
          {message}
        </div>

        {onClose && (
          <button
            type="button"
            className="btn-close"
            aria-label="Đóng"
            onClick={
              onClose
            }
          />
        )}
      </div>
    </div>
  );
}