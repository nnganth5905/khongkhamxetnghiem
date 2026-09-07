import React from 'react';

import AdminCrudPage from '../../components/admin/AdminCrudPage';

import {
  getCustomers,
  createCustomer,
  updateCustomer,
  deleteCustomer,
} from '../../services/adminService';

const columns = [
  {
    key: 'maKhachHang',
    label: 'Mã KH',
    value: (row) =>
      row.maKhachHang ??
      row.MaKhachHang ??
      row.idKhachHang ??
      row.IDKhachHang,
  },
  {
    key: 'hoTen',
    label: 'Họ và tên',
    value: (row) =>
      row.hoTen ??
      row.HoTen ??
      row.fullName ??
      row.FullName,
  },
  {
    key: 'soDienThoai',
    label: 'Số điện thoại',
    value: (row) =>
      row.soDienThoai ??
      row.SoDienThoai ??
      row.phone,
  },
  {
    key: 'email',
    label: 'Email',
    value: (row) =>
      row.email ??
      row.Email,
  },
  {
    key: 'gioiTinh',
    label: 'Giới tính',
    value: (row) =>
      row.gioiTinh ??
      row.GioiTinh,
  },
  {
    key: 'trangThai',
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
    key: 'soDienThoai',
    label: 'Số điện thoại',
    required: true,
    colClass: 'col-md-3',
  },
  {
    key: 'email',
    label: 'Email',
    type: 'email',
    colClass: 'col-md-3',
  },
  {
    key: 'ngaySinh',
    label: 'Ngày sinh',
    type: 'date',
    colClass: 'col-md-3',
  },
  {
    key: 'gioiTinh',
    label: 'Giới tính',
    type: 'select',
    options: [
      {
        value: 'Nam',
        label: 'Nam',
      },
      {
        value: 'Nữ',
        label: 'Nữ',
      },
      {
        value: 'Khác',
        label: 'Khác',
      },
    ],
    colClass: 'col-md-3',
  },
  {
    key: 'diaChi',
    label: 'Địa chỉ',
    colClass: 'col-md-6',
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
    key: 'ghiChu',
    label: 'Ghi chú',
    type: 'textarea',
    rows: 3,
    colClass: 'col-12',
  },
];

export default function QuanLyKhachHang() {
  return (
    <AdminCrudPage
      title="Quản lý khách hàng"
      subtitle="Quản lý hồ sơ khách hàng sử dụng dịch vụ tại Bio Medic Center."
      icon="fa-solid fa-users"
      columns={columns}
      fields={fields}
      loadItems={getCustomers}
      createItem={createCustomer}
      updateItem={updateCustomer}
      deleteItem={deleteCustomer}
      itemId={(row) =>
        row.id ??
        row.idKhachHang ??
        row.IDKhachHang
      }
      searchPlaceholder="Tìm theo mã khách hàng, họ tên, số điện thoại, email..."
      addButtonText="Thêm khách hàng"
      formTitleCreate="Thêm khách hàng"
      formTitleEdit="Cập nhật khách hàng"
    />
  );
}