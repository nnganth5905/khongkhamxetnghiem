import React from 'react';

import AdminCrudPage from '../../components/admin/AdminCrudPage';

import {
  getSpecialties,
  createSpecialty,
  updateSpecialty,
  deleteSpecialty,
} from '../../services/adminService';

const columns = [
  {
    label: 'Mã chuyên khoa',
    value: (row) =>
      row.maChuyenKhoa ??
      row.MaChuyenKhoa ??
      row.idChuyenKhoa ??
      row.IDChuyenKhoa,
  },
  {
    label: 'Tên chuyên khoa',
    value: (row) =>
      row.tenChuyenKhoa ??
      row.TenChuyenKhoa ??
      row.name,
  },
  {
    label: 'Mô tả',
    value: (row) =>
      row.moTa ??
      row.MoTa ??
      '—',
  },
  {
    label: 'Trạng thái',
    type: 'status',
    value: (row) =>
      row.trangThai ??
      row.TrangThai ??
      row.status ??
      'ACTIVE',
  },
];

const fields = [
  {
    key: 'tenChuyenKhoa',
    label: 'Tên chuyên khoa',
    required: true,
    colClass: 'col-md-8',
  },
  {
    key: 'trangThai',
    label: 'Trạng thái',
    type: 'select',
    defaultValue: 'ACTIVE',
    options: [
      {
        value: 'ACTIVE',
        label: 'Hoạt động',
      },
      {
        value: 'INACTIVE',
        label: 'Ngừng hoạt động',
      },
    ],
    colClass: 'col-md-4',
  },
  {
    key: 'moTa',
    label: 'Mô tả',
    type: 'textarea',
    rows: 4,
    colClass: 'col-12',
  },
];

export default function QuanLyChuyenKhoa() {
  return (
    <AdminCrudPage
      title="Quản lý chuyên khoa"
      subtitle="Quản lý danh mục chuyên khoa phục vụ phân công bác sĩ và đặt lịch khám."
      icon="fa-solid fa-heart-pulse"
      columns={columns}
      fields={fields}
      loadItems={getSpecialties}
      createItem={createSpecialty}
      updateItem={updateSpecialty}
      deleteItem={deleteSpecialty}
      itemId={(row) =>
        row.id ??
        row.idChuyenKhoa ??
        row.IDChuyenKhoa
      }
      searchPlaceholder="Tìm mã hoặc tên chuyên khoa..."
      addButtonText="Thêm chuyên khoa"
      formTitleCreate="Thêm chuyên khoa"
      formTitleEdit="Cập nhật chuyên khoa"
    />
  );
}