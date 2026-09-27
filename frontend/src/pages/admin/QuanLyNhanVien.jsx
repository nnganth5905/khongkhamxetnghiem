import React from 'react';

import AdminCrudPage
  from '../../components/admin/AdminCrudPage';

import {
  getEmployees,
  createEmployee,
  updateEmployee,
  deleteEmployee,
} from '../../services/adminService';

// =====================================================
// ROLE LABEL
// =====================================================

function getRoleLabel(
  role,
) {
  switch (
    String(
      role || '',
    ).toUpperCase()
  ) {
    case 'DOCTOR':
      return 'Bác sĩ';

    case 'RECEPTIONIST':
      return 'Lễ tân';

    case 'TECHNICIAN':
      return 'Kỹ thuật viên';

    case 'ADMIN':
      return 'Quản trị viên';

    case 'NURSE':
      return 'Điều dưỡng';

    case 'STAFF':
      return 'Nhân viên';

    default:
      return role || '—';
  }
}

// =====================================================
// COLUMNS
// =====================================================

const columns = [
  {
    key:
      'idNhanVien',

    label:
      'Mã NV',

    value:
      (row) =>
        row.idNhanVien ??
        row.id ??
        '—',
  },

  {
    key:
      'hoTen',

    label:
      'Họ và tên',

    value:
      (row) =>
        row.hoTen ??
        row.fullName ??
        '—',
  },

  {
    key:
      'vaiTro',

    label:
      'Vai trò',

    value:
      (row) =>
        getRoleLabel(
          row.vaiTro ??
          row.role ??
          row.position,
        ),
  },

  {
    key:
      'soDienThoai',

    label:
      'Số điện thoại',

    value:
      (row) =>
        row.soDienThoai ??
        row.phone ??
        '—',
  },

  {
    key:
      'email',

    label:
      'Email',

    value:
      (row) =>
        row.email ??
        '—',
  },

  {
    key:
      'idCoSo',

    label:
      'Cơ sở',

    value:
      (row) =>
        row.idCoSo ??
        row.facilityId ??
        '—',
  },

  {
    key:
      'trangThai',

    label:
      'Trạng thái',

    type:
      'status',

    value:
      (row) =>
        (
          row.trangThai ===
            'INACTIVE'
          ||
          row.status ===
            'no'
        )
          ? 'Đã khóa'
          : 'Hoạt động',
  },
];

// =====================================================
// FIELDS
// =====================================================

const fields = [
  {
    key:
      'hoTen',

    label:
      'Họ và tên',

    required:
      true,

    colClass:
      'col-md-6',

    placeholder:
      'Nhập họ tên nhân viên',
  },

  {
    key:
      'vaiTro',

    label:
      'Vai trò',

    type:
      'select',

    required:
      true,

    options: [
      {
        value:
          'RECEPTIONIST',

        label:
          'Lễ tân',
      },

      {
        value:
          'TECHNICIAN',

        label:
          'Kỹ thuật viên',
      },

      {
        value:
          'STAFF',

        label:
          'Nhân viên',
      },

      {
        value:
          'NURSE',

        label:
          'Điều dưỡng',
      },

      {
        value:
          'DOCTOR',

        label:
          'Bác sĩ',
      },

      {
        value:
          'ADMIN',

        label:
          'Quản trị viên',
      },
    ],

    colClass:
      'col-md-3',
  },

  {
    key:
      'idCoSo',

    label:
      'Mã cơ sở',

    placeholder:
      'VD: CS001',

    colClass:
      'col-md-3',
  },

  {
    key:
      'soDienThoai',

    label:
      'Số điện thoại',

    colClass:
      'col-md-4',
  },

  {
    key:
      'email',

    label:
      'Email',

    type:
      'email',

    colClass:
      'col-md-4',
  },

  {
    key:
      'trangThai',

    label:
      'Trạng thái',

    type:
      'select',

    defaultValue:
      'ACTIVE',

    options: [
      {
        value:
          'ACTIVE',

        label:
          'Đang làm việc',
      },

      {
        value:
          'INACTIVE',

        label:
          'Ngừng làm việc',
      },
    ],

    colClass:
      'col-md-4',
  },
];

// =====================================================
// PAGE
// =====================================================

export default function QuanLyNhanVien() {
  return (
    <AdminCrudPage
      title="Quản lý nhân viên"

      subtitle="Quản lý nhân viên vận hành, lễ tân và kỹ thuật viên trong hệ thống."

      icon="fa-solid fa-users-gear"

      columns={
        columns
      }

      fields={
        fields
      }

      loadItems={
        getEmployees
      }

      createItem={
        createEmployee
      }

      updateItem={
        updateEmployee
      }

      deleteItem={
        deleteEmployee
      }

      idKey="idNhanVien"

      itemId={
        (row) =>
          row.idNhanVien ??
          row.id
      }

      searchPlaceholder="Tìm mã nhân viên, họ tên, vai trò, email..."

      addButtonText="Thêm nhân viên"

      formTitleCreate="Thêm nhân viên"

      formTitleEdit="Cập nhật nhân viên"
    />
  );
}