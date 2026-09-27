import React from 'react';

import AdminCrudPage
  from '../../components/admin/AdminCrudPage';

import {
  getDoctors,
  createDoctor,
  updateDoctor,
  deleteDoctor,
} from '../../services/adminService';

// =====================================================
// COLUMNS
// =====================================================

const columns = [
  {
    key:
      'id',

    label:
      'Mã bác sĩ',

    value:
      (row) =>
        row.id ??
        row.idBacSi ??
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
      'tenChuyenKhoa',

    label:
      'Chuyên khoa',

    value:
      (row) =>
        row.tenChuyenKhoa ??
        row.specialtyName ??
        row.idChuyenKhoa ??
        '—',
  },

  {
    key:
      'hocVi',

    label:
      'Học vị',

    value:
      (row) =>
        row.hocVi ??
        row.degree ??
        '—',
  },

  {
    key:
      'chucDanh',

    label:
      'Chức danh',

    value:
      (row) =>
        row.chucDanh ??
        row.title ??
        '—',
  },

  {
    key:
      'coSoId',

    label:
      'Cơ sở',

    value:
      (row) =>
        row.coSoId ??
        row.facilityId ??
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
      'soSao',

    label:
      'Đánh giá',

    value:
      (row) =>
        row.soSao !==
        undefined
          ? `${row.soSao} ★`
          : '—',
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
          row.trangThai ??
          row.status
        ) === 'active'
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
      'hoTen',

    label:
      'Họ và tên',

    required:
      true,

    colClass:
      'col-md-6',
  },

  {
    key:
      'idChuyenKhoa',

    label:
      'Mã chuyên khoa',

    required:
      true,

    colClass:
      'col-md-3',

    placeholder:
      'VD: CK001',
  },

  {
    key:
      'hocVi',

    label:
      'Học vị',

    placeholder:
      'Bác sĩ, Thạc sĩ, Tiến sĩ...',

    colClass:
      'col-md-3',
  },

  {
    key:
      'chucDanh',

    label:
      'Chức danh',

    placeholder:
      'Bác sĩ chuyên khoa I...',

    colClass:
      'col-md-6',
  },

  {
    /*
     * FIX:
     * DB là CoSoID,
     * không phải IDPhong.
     */
    key:
      'coSoId',

    label:
      'Mã cơ sở',

    placeholder:
      'VD: CS001',

    colClass:
      'col-md-3',
  },

  {
    key:
      'namKinhNghiem',

    label:
      'Năm kinh nghiệm',

    type:
      'number',

    min:
      0,

    max:
      100,

    colClass:
      'col-md-3',
  },

  {
    key:
      'hinhAnh',

    label:
      'URL hình ảnh',

    colClass:
      'col-md-8',
  },

  {
    key:
      'trangThai',

    label:
      'Trạng thái',

    type:
      'select',

    defaultValue:
      'active',

    options: [
      {
        value:
          'active',

        label:
          'Đang làm việc',
      },

      {
        value:
          'inactive',

        label:
          'Ngừng làm việc',
      },
    ],

    colClass:
      'col-md-4',
  },

  {
    key:
      'gioiThieu',

    label:
      'Giới thiệu',

    type:
      'textarea',

    rows:
      4,

    colClass:
      'col-12',
  },
];

// =====================================================
// PAGE
// =====================================================

export default function QuanLyBacSi() {
  return (
    <AdminCrudPage
      title="Quản lý bác sĩ"

      subtitle="Quản lý hồ sơ bác sĩ, chuyên khoa và trạng thái làm việc."

      icon="fa-solid fa-user-doctor"

      columns={
        columns
      }

      fields={
        fields
      }

      loadItems={
        getDoctors
      }

      createItem={
        createDoctor
      }

      updateItem={
        updateDoctor
      }

      deleteItem={
        deleteDoctor
      }

      idKey="id"

      itemId={
        (row) =>
          row.id ??
          row.idBacSi
      }

      searchPlaceholder="Tìm mã bác sĩ, họ tên, chuyên khoa..."

      addButtonText="Thêm bác sĩ"

      formTitleCreate="Thêm bác sĩ"

      formTitleEdit="Cập nhật bác sĩ"
    />
  );
}