import React from 'react';

import AdminCrudPage from '../../components/admin/AdminCrudPage';

import {
  getTechnicians,
  createTechnician,
  updateTechnician,
  deleteTechnician,
} from '../../services/adminService';

const columns = [
  {
    label: 'Mã KTV',
    value: (row) =>
      row.maKTV ??
      row.MaKTV ??
      row.idNhanVien ??
      row.IDNhanVien,
  },
  {
    label: 'Họ và tên',
    value: (row) =>
      row.hoTen ??
      row.HoTen ??
      row.tenNhanVien ??
      row.TenNhanVien,
  },
  {
    label: 'Cơ sở',
    value: (row) =>
      row.tenCoSo ??
      row.TenCoSo ??
      row.idCoSo ??
      row.IDCoSo ??
      '—',
  },
  {
    label: 'Số điện thoại',
    value: (row) =>
      row.soDienThoai ??
      row.SoDienThoai ??
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
    key: 'idCoSo',
    label: 'Mã cơ sở',
    required: true,
    colClass: 'col-md-3',
  },
  {
    key: 'trinhDo',
    label: 'Trình độ',
    placeholder: 'KTV xét nghiệm...',
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

export default function QuanLyKTV() {
  return (
    <AdminCrudPage
      title="Quản lý kỹ thuật viên"
      subtitle="Quản lý kỹ thuật viên phòng xét nghiệm và trạng thái làm việc."
      icon="fa-solid fa-microscope"
      columns={columns}
      fields={fields}
      loadItems={getTechnicians}
      createItem={createTechnician}
      updateItem={updateTechnician}
      deleteItem={deleteTechnician}
      itemId={(row) =>
        row.id ??
        row.idNhanVien ??
        row.IDNhanVien
      }
      searchPlaceholder="Tìm mã KTV, họ tên, cơ sở, email..."
      addButtonText="Thêm kỹ thuật viên"
      formTitleCreate="Thêm kỹ thuật viên"
      formTitleEdit="Cập nhật kỹ thuật viên"
    />
  );
}