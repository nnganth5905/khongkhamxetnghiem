import React from 'react';

import AdminCrudPage
  from '../../components/admin/AdminCrudPage';

import {
  getSpecialties,
  createSpecialty,
  updateSpecialty,
  deleteSpecialty,
} from '../../services/adminService';

const columns = [
  {
    key: 'idChuyenKhoa',

    label: 'Mã chuyên khoa',

    value: (row) =>
      row.idChuyenKhoa ??
      row.maChuyenKhoa ??
      row.id ??
      '—',
  },

  {
    key: 'tenChuyenKhoa',

    label: 'Tên chuyên khoa',

    value: (row) =>
      row.tenChuyenKhoa ??
      row.name ??
      '—',
  },

  {
    key: 'moTa',

    label: 'Mô tả',

    value: (row) =>
      row.moTa ??
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
        ? 'Ngừng hoạt động'
        : 'Hoạt động',
  },
];

const fields = [
  {
    key: 'tenChuyenKhoa',

    label: 'Tên chuyên khoa',

    required: true,

    colClass: 'col-md-8',
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
    key: 'moTa',

    label: 'Mô tả',

    type: 'textarea',

    rows: 4,

    colClass: 'col-12',
  },
];

export default function QuanLyChuyenKhoa() {
  return (
    <AdminCrudPage
      title="Quản lý chuyên khoa"

      subtitle="Quản lý danh mục chuyên khoa phục vụ phân công bác sĩ và đặt lịch khám."

      icon="fa-solid fa-heart-pulse"

      columns={columns}

      fields={fields}

      loadItems={getSpecialties}

      createItem={createSpecialty}

      updateItem={updateSpecialty}

      deleteItem={deleteSpecialty}

      idKey="idChuyenKhoa"

      itemId={(row) =>
        row.idChuyenKhoa ??
        row.id
      }

      searchPlaceholder="Tìm mã hoặc tên chuyên khoa..."

      addButtonText="Thêm chuyên khoa"

      formTitleCreate="Thêm chuyên khoa"

      formTitleEdit="Cập nhật chuyên khoa"
    />
  );
}