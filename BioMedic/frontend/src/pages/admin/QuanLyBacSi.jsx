import React from 'react';

import AdminCrudPage from '../../components/admin/AdminCrudPage';

import {
  getDoctors,
  createDoctor,
  updateDoctor,
  deleteDoctor,
} from '../../services/adminService';

const columns = [
  {
    label: 'Mã bác sĩ',
    value: (row) =>
      row.maBacSi ??
      row.MaBacSi ??
      row.idBacSi ??
      row.IDBacSi,
  },
  {
    label: 'Họ và tên',
    value: (row) =>
      row.hoTen ??
      row.HoTen ??
      row.tenBacSi ??
      row.TenBacSi,
  },
  {
    label: 'Chuyên khoa',
    value: (row) =>
      row.tenChuyenKhoa ??
      row.TenChuyenKhoa ??
      row.idChuyenKhoa ??
      row.IDChuyenKhoa ??
      '—',
  },
  {
    label: 'Học vị',
    value: (row) =>
      row.hocVi ??
      row.HocVi ??
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
    key: 'idChuyenKhoa',
    label: 'Mã chuyên khoa',
    required: true,
    colClass: 'col-md-3',
  },
  {
    key: 'hocVi',
    label: 'Học vị',
    placeholder: 'BS, ThS, TS...',
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
    key: 'idPhong',
    label: 'Mã phòng mặc định',
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
    key: 'gioiThieu',
    label: 'Giới thiệu',
    type: 'textarea',
    rows: 4,
    colClass: 'col-12',
  },
];

export default function QuanLyBacSi() {
  return (
    <AdminCrudPage
      title="Quản lý bác sĩ"
      subtitle="Quản lý hồ sơ bác sĩ, chuyên khoa và trạng thái làm việc."
      icon="fa-solid fa-user-doctor"
      columns={columns}
      fields={fields}
      loadItems={getDoctors}
      createItem={createDoctor}
      updateItem={updateDoctor}
      deleteItem={deleteDoctor}
      itemId={(row) =>
        row.id ??
        row.idBacSi ??
        row.IDBacSi
      }
      searchPlaceholder="Tìm mã bác sĩ, họ tên, chuyên khoa..."
      addButtonText="Thêm bác sĩ"
      formTitleCreate="Thêm bác sĩ"
      formTitleEdit="Cập nhật bác sĩ"
    />
  );
}