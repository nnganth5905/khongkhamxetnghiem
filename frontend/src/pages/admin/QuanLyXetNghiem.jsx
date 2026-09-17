import React from 'react';

import AdminCrudPage from '../../components/admin/AdminCrudPage';

import {
  getAdminTests,
  createAdminTest,
  updateAdminTest,
  deleteAdminTest,
} from '../../services/adminService';

const columns = [
  {
    label: 'Mã XN',
    value: (row) =>
      row.maXetNghiem ??
      row.MaXetNghiem ??
      row.id ??
      row.idXetNghiem ??
      row.IDXetNghiem,
  },
  {
    label: 'Tên xét nghiệm',
    value: (row) =>
      row.tenXetNghiem ??
      row.TenXetNghiem ??
      row.name,
  },
  {
    label: 'Loại xét nghiệm',
    value: (row) =>
      row.tenLoai ??
      row.TenLoai ??
      row.idLoaiXetNghiem ??
      row.IDLoaiXetNghiem ??
      '—',
  },
  {
    label: 'Loại mẫu',
    value: (row) =>
      row.loaiMau ??
      row.LoaiMau ??
      '—',
  },
  {
    label: 'Giá',
    type: 'money',
    value: (row) =>
      row.gia ??
      row.Gia ??
      row.price,
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
    key: 'tenXetNghiem',
    label: 'Tên xét nghiệm',
    required: true,
    colClass: 'col-md-6',
  },
  {
    key: 'idLoaiXetNghiem',
    label: 'Mã loại xét nghiệm',
    required: true,
    colClass: 'col-md-3',
  },
  {
    key: 'loaiMau',
    label: 'Loại mẫu',
    placeholder: 'Máu, huyết thanh, nước tiểu...',
    colClass: 'col-md-3',
  },
  {
    key: 'gia',
    label: 'Giá dịch vụ',
    type: 'number',
    min: 0,
    step: 1000,
    colClass: 'col-md-4',
  },
  {
    key: 'thoiGianTraKetQua',
    label: 'Thời gian trả kết quả',
    placeholder: 'Ví dụ: 2 giờ / 24 giờ',
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
        label: 'Đang cung cấp',
      },
      {
        value: 'INACTIVE',
        label: 'Ngừng cung cấp',
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
  {
    key: 'chuanBi',
    label: 'Hướng dẫn chuẩn bị',
    type: 'textarea',
    rows: 3,
    colClass: 'col-12',
  },
];

export default function QuanLyXetNghiem() {
  return (
    <AdminCrudPage
      title="Quản lý xét nghiệm"
      subtitle="Quản lý danh mục dịch vụ xét nghiệm, giá và thông tin thực hiện."
      icon="fa-solid fa-flask-vial"
      columns={columns}
      fields={fields}
      loadItems={getAdminTests}
      createItem={createAdminTest}
      updateItem={updateAdminTest}
      deleteItem={deleteAdminTest}
      itemId={(row) =>
        row.id ??
        row.idXetNghiem ??
        row.IDXetNghiem
      }
      searchPlaceholder="Tìm mã xét nghiệm, tên xét nghiệm, loại xét nghiệm..."
      addButtonText="Thêm xét nghiệm"
      formTitleCreate="Thêm xét nghiệm"
      formTitleEdit="Cập nhật xét nghiệm"
    />
  );
}