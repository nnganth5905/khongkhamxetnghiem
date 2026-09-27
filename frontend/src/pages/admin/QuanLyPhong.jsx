import React from 'react';

import AdminCrudPage
  from '../../components/admin/AdminCrudPage';

import {
  getRooms,
  createRoom,
  updateRoom,
  deleteRoom,
} from '../../services/adminService';

const getRoomTypeText = (type) => {
  switch (
    String(type || '').toUpperCase()
  ) {
    case 'EXAMINATION':
      return 'Phòng khám';

    case 'SAMPLING':
      return 'Phòng lấy mẫu';

    case 'LAB':
      return 'Phòng xét nghiệm';

    case 'CONSULTATION':
      return 'Phòng tư vấn';

    case 'OTHER':
      return 'Khác';

    default:
      return type || '—';
  }
};

const getRoomStatusText = (
  status,
) => {
  switch (
    String(status || '').toUpperCase()
  ) {
    case 'AVAILABLE':
    case 'ACTIVE':
      return 'Sẵn sàng';

    case 'MAINTENANCE':
      return 'Bảo trì';

    case 'INACTIVE':
      return 'Ngừng sử dụng';

    default:
      return status || '—';
  }
};

const columns = [
  {
    key: 'idPhong',

    label: 'Mã phòng',

    value: (row) =>
      row.idPhong ??
      row.maPhong ??
      row.id ??
      '—',
  },

  {
    key: 'tenPhong',

    label: 'Tên phòng',

    value: (row) =>
      row.tenPhong ??
      row.name ??
      '—',
  },

  {
    key: 'loaiPhong',

    label: 'Loại phòng',

    value: (row) =>
      getRoomTypeText(
        row.loaiPhong
      ),
  },

  {
    key: 'tenCoSo',

    label: 'Cơ sở',

    value: (row) =>
      row.tenCoSo ??
      row.idCoSo ??
      '—',
  },

  {
    key: 'tenChuyenKhoa',

    label: 'Chuyên khoa',

    value: (row) =>
      row.tenChuyenKhoa ??
      row.idChuyenKhoa ??
      '—',
  },

  {
    key: 'tang',

    label: 'Tầng',

    value: (row) =>
      row.tang ??
      '—',
  },

  {
    key: 'trangThai',

    label: 'Trạng thái',

    type: 'status',

    value: (row) =>
      getRoomStatusText(
        row.trangThai ??
        row.status
      ),
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
        value: 'CONSULTATION',
        label: 'Phòng tư vấn',
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

    placeholder: 'VD: CS001',

    colClass: 'col-md-3',
  },

  {
    key: 'idChuyenKhoa',

    label: 'Mã chuyên khoa',

    placeholder: 'VD: CK001',

    colClass: 'col-md-4',
  },

  {
    key: 'tang',

    label: 'Tầng',

    placeholder: 'VD: Tầng 1',

    colClass: 'col-md-4',
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

    colClass: 'col-md-4',
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

      idKey="idPhong"

      itemId={(row) =>
        row.idPhong ??
        row.id
      }

      searchPlaceholder="Tìm mã phòng, tên phòng, loại phòng, cơ sở..."

      addButtonText="Thêm phòng"

      formTitleCreate="Thêm phòng"

      formTitleEdit="Cập nhật phòng"
    />
  );
}