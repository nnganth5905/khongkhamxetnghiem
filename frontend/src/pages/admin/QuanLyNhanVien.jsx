import React from 'react';

import AdminCrudPage from '../../components/admin/AdminCrudPage';

import {
  getEmployees,
  createEmployee,
  updateEmployee,
  deleteEmployee,
} from '../../services/adminService';

const ROLE_LABELS = {
  bacsi: 'Bác sĩ',
  letan: 'Lễ tân',
  ktv: 'Kỹ thuật viên',
  admin: 'Quản trị viên',
  dieu_duong: 'Điều dưỡng',
  khac: 'Nhân viên',
};

const STATUS_LABELS = {
  yes: 'Đang làm việc',
  no: 'Ngừng làm việc',
};

const columns = [
  {
    label: 'Mã NV',
    value: (row) =>
      row.idNhanVien ??
      row.IDNhanVien ??
      row.id ??
      '—',
  },
  {
    label: 'Họ và tên',
    value: (row) =>
      row.hoTen ??
      row.HoTen ??
      row.fullName ??
      row.FullName ??
      row.name ??
      '—',
  },
  {
    label: 'Vai trò',
    value: (row) => {
      const role =
        row.vaiTro ??
        row.VaiTro ??
        row.role ??
        row.position;

      return ROLE_LABELS[String(role ?? '').toLowerCase()] ?? role ?? '—';
    },
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
    label: 'Cơ sở',
    value: (row) =>
      row.idCoSo ??
      row.facilityId ??
      '—',
  },
  {
    label: 'Trạng thái',
    value: (row) => {
      const status =
        row.trangThai ??
        row.TrangThai ??
        row.status;

      return STATUS_LABELS[String(status ?? '').toLowerCase()] ?? status ?? '—';
    },
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
    defaultValue: 'khac',
    options: [
      {
        value: 'letan',
        label: 'Lễ tân',
      },
      {
        value: 'ktv',
        label: 'Kỹ thuật viên',
      },
      {
        value: 'dieu_duong',
        label: 'Điều dưỡng',
      },
      {
        value: 'khac',
        label: 'Nhân viên',
      },
      {
        value: 'admin',
        label: 'Quản trị viên',
      },
      {
        value: 'bacsi',
        label: 'Bác sĩ',
      },
    ],
    colClass: 'col-md-3',
  },
  {
    key: 'idCoSo',
    label: 'Mã cơ sở',
    defaultValue: 'CS001',
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
    key: 'trangThai',
    label: 'Trạng thái',
    type: 'select',
    defaultValue: 'yes',
    options: [
      {
        value: 'yes',
        label: 'Đang làm việc',
      },
      {
        value: 'no',
        label: 'Ngừng làm việc',
      },
    ],
    colClass: 'col-md-4',
  },
];

export default function QuanLyNhanVien() {
  return (
    <AdminCrudPage
      title="Quản lý nhân viên"
      subtitle="Quản lý nhân viên vận hành, lễ tân, kỹ thuật viên và các vị trí khác trong hệ thống."
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
