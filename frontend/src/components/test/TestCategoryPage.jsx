// src/components/test/TestCategoryPage.jsx
import React from 'react';

export default function TestCategoryPage({ slug, title, description, icon }) {
  return (
    <div className="container py-4">
      <div className="d-flex align-items-center gap-3 mb-3">
        {icon && <i className={`${icon} fa-2x text-primary`}></i>}
        <h2 className="mb-0">{title}</h2>
      </div>

      <p className="text-muted">{description}</p>

      {/* Khu vực bảng giá / danh sách xét nghiệm sau này render ở đây */}
      <div className="card p-3 shadow-sm mt-4">
        <h5 className="card-title">Danh mục các gói xét nghiệm ({slug})</h5>
        <p className="text-secondary">
          Nội dung chi tiết các gói xét nghiệm thuộc danh mục này sẽ được hiển thị ở đây.
        </p>
      </div>
    </div>
  );
}