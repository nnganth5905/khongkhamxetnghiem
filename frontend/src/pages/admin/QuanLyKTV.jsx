import React from 'react';

import AdminCrudPage
  from '../../components/admin/AdminCrudPage';

import {
  getTechnicians,
  createTechnician,
  updateTechnician,
  deleteTechnician,
} from '../../services/adminService';

const columns = [
  {
    key: 'idNhanVien',
    label: 'Mã KTV',

    value: (row) =>
      row.idNhanVien ??
      row.maKTV ??
      row.id ??
      '—',
  },

  {
    key: 'hoTen',
    label: 'Họ và tên',

    value: (row) =>
      row.hoTen ??
      '—',
  },

  {
    key: 'idCoSo',
    label: 'Cơ sở',

    value: (row) =>
      row.tenCoSo ??
      row.idCoSo ??
      '—',
  },

  {
    key: 'soDienThoai',
    label: 'Số điện thoại',

    value: (row) =>
      row.soDienThoai ??
      '—',
  },

  {
    key: 'email',
    label: 'Email',

    value: (row) =>
      row.email ??
      '—',
  },

  {
    key: 'trangThai',
    label: 'Trạng thái',
    type: 'status',

    value: (row) =>
      (
        row.trangThai === 'INACTIVE'
        ||
        row.status === 'no'
      )
        ? 'Đã khóa'
        : 'Hoạt động',
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
    placeholder: 'VD: CS001',
    colClass: 'col-md-6',
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

      idKey="idNhanVien"

      itemId={(row) =>
        row.idNhanVien ??
        row.id
      }

      searchPlaceholder="Tìm mã KTV, họ tên, cơ sở, email..."

      addButtonText="Thêm kỹ thuật viên"

      formTitleCreate="Thêm kỹ thuật viên"

      formTitleEdit="Cập nhật kỹ thuật viên"
    />
  );
}