import React from 'react';

import AdminCrudPage from '../../components/admin/AdminCrudPage';

import {
  getRooms,
  createRoom,
  updateRoom,
  deleteRoom,
} from '../../services/adminService';

const columns = [
  {
    label: 'Mã phòng',
    value: (row) =>
      row.maPhong ??
      row.MaPhong ??
      row.idPhong ??
      row.IDPhong,
  },
  {
    label: 'Tên phòng',
    value: (row) =>
      row.tenPhong ??
      row.TenPhong ??
      row.name,
  },
  {
    label: 'Loại phòng',
    value: (row) =>
      row.loaiPhong ??
      row.LoaiPhong ??
      '—',
  },
  {
    label: 'Cơ sở',
    value: (row) =>
      row.tenCoSo ??
      row.TenCoSo ??
      row.idCoSo ??
      row.IDCoSo ??
      '—',
  },
  {
    label: 'Tầng',
    value: (row) =>
      row.tang ??
      row.Tang ??
      '—',
  },
  {
    label: 'Trạng thái',
    type: 'status',
    value: (row) =>
      row.trangThai ??
      row.TrangThai ??
      row.status ??
      'AVAILABLE',
  },
];

const fields = [
  {
    key: 'tenPhong',
    label: 'Tên phòng',
    required: true,
    colClass: 'col-md-6',
  },
  {
    key: 'loaiPhong',
    label: 'Loại phòng',
    type: 'select',
    required: true,
    options: [
      {
        value: 'EXAMINATION',
        label: 'Phòng khám',
      },
      {
        value: 'SAMPLING',
        label: 'Phòng lấy mẫu',
      },
      {
        value: 'LAB',
        label: 'Phòng xét nghiệm',
      },
      {
        value: 'OTHER',
        label: 'Khác',
      },
    ],
    colClass: 'col-md-3',
  },
  {
    key: 'idCoSo',
    label: 'Mã cơ sở',
    required: true,
    colClass: 'col-md-3',
  },
  {
    key: 'tang',
    label: 'Tầng',
    colClass: 'col-md-3',
  },
  {
    key: 'sucChua',
    label: 'Sức chứa',
    type: 'number',
    min: 1,
    colClass: 'col-md-3',
  },
  {
    key: 'trangThai',
    label: 'Trạng thái',
    type: 'select',
    defaultValue: 'AVAILABLE',
    options: [
      {
        value: 'AVAILABLE',
        label: 'Sẵn sàng',
      },
      {
        value: 'MAINTENANCE',
        label: 'Bảo trì',
      },
      {
        value: 'INACTIVE',
        label: 'Ngừng sử dụng',
      },
    ],
    colClass: 'col-md-3',
  },
  {
    key: 'ghiChu',
    label: 'Ghi chú',
    type: 'textarea',
    colClass: 'col-12',
  },
];

export default function QuanLyPhong() {
  return (
    <AdminCrudPage
      title="Quản lý phòng"
      subtitle="Quản lý phòng khám, phòng lấy mẫu và khu vực xét nghiệm theo từng cơ sở."
      icon="fa-solid fa-door-open"
      columns={columns}
      fields={fields}
      loadItems={getRooms}
      createItem={createRoom}
      updateItem={updateRoom}
      deleteItem={deleteRoom}
      itemId={(row) =>
        row.id ??
        row.idPhong ??
        row.IDPhong
      }
      searchPlaceholder="Tìm mã phòng, tên phòng, loại phòng, cơ sở..."
      addButtonText="Thêm phòng"
      formTitleCreate="Thêm phòng"
      formTitleEdit="Cập nhật phòng"
    />
  );
}