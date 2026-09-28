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
      row.IDXetNghiem ??
      '—',
  },
  {
    label: 'Tên xét nghiệm',
    value: (row) =>
      row.tenXetNghiem ??
      row.TenXetNghiem ??
      row.name ??
      '—',
  },
  {
    label: 'Chuyên khoa',
    value: (row) =>
      row.idLoaiXetNghiem ??
      row.specialtyId ??
      row.ChuyenKhoaID ??
      row.chuyenKhoaId ??
      '—',
  },
  {
    label: 'Loại xét nghiệm',
    value: (row) =>
      row.loai ??
      row.Loai ??
      row.type ??
      '—',
  },
  {
    label: 'Loại mẫu',
    value: (row) =>
      row.loaiMau ??
      row.LoaiMau ??
      row.sampleType ??
      '—',
  },
  {
    label: 'Giá',
    type: 'money',
    value: (row) =>
      row.gia ??
      row.Gia ??
      row.price ??
      0,
  },
  {
    label: 'Thời gian dự kiến',
    value: (row) => {
      const minutes =
        row.expectedMinutes ??
        row.thoiGianDuKienPhut ??
        row.ThoiGianDuKienPhut;

      return minutes !== null && minutes !== undefined && minutes !== ''
        ? `${minutes} phút`
        : '—';
    },
  },
  {
    label: 'Trạng thái',
    type: 'status',
    value: (row) =>
      row.trangThai ??
      row.TrangThai ??
      row.status ??
      'yes',
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
    // adminService hiện map field này sang specialtyId của backend.
    key: 'idLoaiXetNghiem',
    label: 'Mã chuyên khoa',
    required: true,
    placeholder: 'Ví dụ: CK001',
    colClass: 'col-md-3',
  },
  {
    key: 'loai',
    label: 'Loại xét nghiệm',
    placeholder: 'Ví dụ: Sinh hóa, Huyết học...',
    colClass: 'col-md-3',
  },
  {
    key: 'loaiMau',
    label: 'Loại mẫu',
    placeholder: 'Máu, huyết thanh, nước tiểu...',
    colClass: 'col-md-4',
  },
  {
    key: 'gia',
    label: 'Giá dịch vụ',
    type: 'number',
    min: 0,
    step: 1000,
    defaultValue: 0,
    colClass: 'col-md-4',
  },
  {
    key: 'expectedMinutes',
    label: 'Thời gian dự kiến (phút)',
    type: 'number',
    min: 1,
    step: 1,
    placeholder: 'Ví dụ: 120',
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
        label: 'Đang cung cấp',
      },
      {
        value: 'no',
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
      searchPlaceholder="Tìm mã xét nghiệm, tên xét nghiệm, chuyên khoa..."
      addButtonText="Thêm xét nghiệm"
      formTitleCreate="Thêm xét nghiệm"
      formTitleEdit="Cập nhật xét nghiệm"
    />
  );
}
