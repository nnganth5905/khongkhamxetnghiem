import React from 'react';

export default function Loading({
  text = 'Đang tải...',
  fullscreen = false,
}) {
  return (
    <div
      className={
        fullscreen
          ? 'app-loading-overlay'
          : 'app-loading'
      }
    >
      <div className="text-center">
        <div
          className="spinner-border text-primary mb-3"
          role="status"
        >
          <span className="visually-hidden">
            Loading...
          </span>
        </div>

        <div className="text-secondary">
          {text}
        </div>
      </div>
    </div>
  );
}