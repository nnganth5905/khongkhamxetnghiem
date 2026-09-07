import React from 'react';

import AdminCrudPage from '../../components/admin/AdminCrudPage';

import {
  getEmployees,
  createEmployee,
  updateEmployee,
  deleteEmployee,
} from '../../services/adminService';

const columns = [
  {
    label: 'Mã NV',
    value: (row) =>
      row.maNhanVien ??
      row.MaNhanVien ??
      row.idNhanVien ??
      row.IDNhanVien,
  },
  {
    label: 'Họ và tên',
    value: (row) =>
      row.hoTen ??
      row.HoTen ??
      row.fullName ??
      row.FullName,
  },
  {
    label: 'Vai trò',
    value: (row) =>
      row.vaiTro ??
      row.VaiTro ??
      row.role ??
      '—',
  },
  {
    label: 'Số điện thoại',
    value: (row) =>
      row.soDienThoai ??
      row.SoDienThoai ??
      row.phone ??
      '—',
  },
  {
    label: 'Email',
    value: (row) =>
      row.email ??
      row.Email ??
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
    key: 'hoTen',
    label: 'Họ và tên',
    required: true,
    colClass: 'col-md-6',
  },
  {
    key: 'vaiTro',
    label: 'Vai trò',
    type: 'select',
    required: true,
    options: [
      {
        value: 'RECEPTIONIST',
        label: 'Lễ tân',
      },
      {
        value: 'TECHNICIAN',
        label: 'Kỹ thuật viên',
      },
      {
        value: 'STAFF',
        label: 'Nhân viên',
      },
    ],
    colClass: 'col-md-3',
  },
  {
    key: 'idCoSo',
    label: 'Mã cơ sở',
    colClass: 'col-md-3',
  },
  {
    key: 'soDienThoai',
    label: 'Số điện thoại',
    colClass: 'col-md-4',
  },
  {
    key: 'email',
    label: 'Email',
    type: 'email',
    colClass: 'col-md-4',
  },
  {
    key: 'ngayVaoLam',
    label: 'Ngày vào làm',
    type: 'date',
    colClass: 'col-md-4',
  },
  {
    key: 'trangThai',
    label: 'Trạng thái',
    type: 'select',
    defaultValue: 'ACTIVE',
    options: [
      {
        value: 'ACTIVE',
        label: 'Đang làm việc',
      },
      {
        value: 'INACTIVE',
        label: 'Ngừng làm việc',
      },
    ],
    colClass: 'col-md-4',
  },
  {
    key: 'ghiChu',
    label: 'Ghi chú',
    type: 'textarea',
    colClass: 'col-12',
  },
];

export default function QuanLyNhanVien() {
  return (
    <AdminCrudPage
      title="Quản lý nhân viên"
      subtitle="Quản lý nhân viên vận hành và lễ tân trong hệ thống."
      icon="fa-solid fa-users-gear"
      columns={columns}
      fields={fields}
      loadItems={getEmployees}
      createItem={createEmployee}
      updateItem={updateEmployee}
      deleteItem={deleteEmployee}
      itemId={(row) =>
        row.id ??
        row.idNhanVien ??
        row.IDNhanVien
      }
      searchPlaceholder="Tìm mã nhân viên, họ tên, vai trò, email..."
      addButtonText="Thêm nhân viên"
      formTitleCreate="Thêm nhân viên"
      formTitleEdit="Cập nhật nhân viên"
    />
  );
}