import React from 'react';

import AdminCrudPage
  from '../../components/admin/AdminCrudPage';

import {
  getCustomers,
  createCustomer,
  updateCustomer,
  deleteCustomer,
} from '../../services/adminService';

// =====================================================
// COLUMNS
// =====================================================

const columns = [
  {
    key:
      'idKhachHang',

    label:
      'Mã KH',

    value:
      (row) =>
        row.idKhachHang ??
        row.id ??
        '—',
  },

  {
    key:
      'tenKhachHang',

    label:
      'Họ và tên',

    value:
      (row) =>
        row.tenKhachHang ??
        '—',
  },

  {
    key:
      'soDienThoai',

    label:
      'Số điện thoại',

    value:
      (row) =>
        row.soDienThoai ??
        '—',
  },

  {
    key:
      'cccd',

    label:
      'CCCD/CMND',

    value:
      (row) =>
        row.cccd ??
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
      'gioiTinh',

    label:
      'Giới tính',

    value:
      (row) => {
        switch (
          String(
            row.gioiTinh ||
            '',
          ).toLowerCase()
        ) {
          case 'nam':
            return 'Nam';

          case 'nu':
          case 'nữ':
            return 'Nữ';

          case 'khac':
          case 'khác':
            return 'Khác';

          default:
            return '—';
        }
      },
  },

  {
    key:
      'status',

    label:
      'Trạng thái',

    type:
      'status',

    value:
      (row) =>
        row.status ===
        'yes'
          ? 'Hoạt động'
          : 'Đã khóa',
  },
];

// =====================================================
// FIELDS
// =====================================================

const fields = [
  {
    key:
      'tenKhachHang',

    label:
      'Họ và tên',

    required:
      true,

    colClass:
      'col-md-6',

    placeholder:
      'Nhập họ và tên',
  },

  {
    key:
      'soDienThoai',

    label:
      'Số điện thoại',

    required:
      true,

    colClass:
      'col-md-3',

    placeholder:
      'Nhập số điện thoại',
  },

  {
    key:
      'ngaySinh',

    label:
      'Ngày sinh',

    type:
      'date',

    colClass:
      'col-md-3',
  },

  {
    key:
      'cccd',

    label:
      'CCCD/CMND',

    colClass:
      'col-md-4',

    placeholder:
      'Nhập CCCD/CMND',
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

    placeholder:
      'example@gmail.com',
  },

  {
    key:
      'gioiTinh',

    label:
      'Giới tính',

    type:
      'select',

    options: [
      {
        value:
          'nam',

        label:
          'Nam',
      },

      {
        value:
          'nu',

        label:
          'Nữ',
      },

      {
        value:
          'khac',

        label:
          'Khác',
      },
    ],

    colClass:
      'col-md-4',
  },

  {
    key:
      'diaChi',

    label:
      'Địa chỉ',

    colClass:
      'col-md-8',

    placeholder:
      'Nhập địa chỉ',
  },

  {
    key:
      'status',

    label:
      'Trạng thái',

    type:
      'select',

    defaultValue:
      'yes',

    options: [
      {
        value:
          'yes',

        label:
          'Hoạt động',
      },

      {
        value:
          'no',

        label:
          'Ngừng hoạt động',
      },
    ],

    colClass:
      'col-md-4',
  },
];

// =====================================================
// PAGE
// =====================================================

export default function QuanLyKhachHang() {
  return (
    <AdminCrudPage
      title="Quản lý khách hàng"

      subtitle="Quản lý hồ sơ khách hàng sử dụng dịch vụ tại Bio Medic Center."

      columns={
        columns
      }

      fields={
        fields
      }

      loadItems={
        getCustomers
      }

      createItem={
        createCustomer
      }

      updateItem={
        updateCustomer
      }

      deleteItem={
        deleteCustomer
      }

      idKey="idKhachHang"

      itemId={
        (row) =>
          row.idKhachHang ??
          row.id
      }

      searchPlaceholder="Tìm theo mã khách hàng, họ tên, số điện thoại, CCCD, email..."

      addButtonText="Thêm khách hàng"

      formTitleCreate="Thêm khách hàng"

      formTitleEdit="Cập nhật khách hàng"
    />
  );
}