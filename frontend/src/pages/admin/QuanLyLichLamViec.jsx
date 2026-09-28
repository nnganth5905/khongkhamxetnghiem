import React from 'react';

import AdminCrudPage
  from '../../components/admin/AdminCrudPage';

import {
  getWorkSchedules,
  createWorkSchedule,
  updateWorkSchedule,
  deleteWorkSchedule,
} from '../../services/adminService';

const getShiftText = (shift) => {
  switch (
    String(shift || '').toUpperCase()
  ) {
    case 'MORNING':
      return 'Ca sáng';

    case 'AFTERNOON':
      return 'Ca chiều';

    case 'EVENING':
      return 'Ca tối';

    case 'CUSTOM':
    case 'FULL_DAY':
      return 'Tùy chỉnh';

    default:
      return shift || '—';
  }
};

const columns = [
  {
    key: 'idLichLamViec',

    label: 'Mã lịch',

    value: (row) =>
      row.idLichLamViec ??
      row.maLich ??
      row.id ??
      '—',
  },

  {
    key: 'tenBacSi',

    label: 'Bác sĩ',

    value: (row) =>
      row.tenBacSi ??
      row.tenNhanSu ??
      (
        row.idBacSi
          ? `BS #${row.idBacSi}`
          : '—'
      ),
  },

  {
    key: 'ngayLamViec',

    label: 'Ngày làm việc',

    value: (row) =>
      row.ngayLamViec ??
      '—',
  },

  {
    key: 'caLamViec',

    label: 'Ca',

    value: (row) =>
      getShiftText(
        row.caLamViec
      ),
  },

  {
    key: 'thoiGian',

    label: 'Thời gian',

    value: (row) => {
      const start =
        row.gioBatDau ??
        '—';

      const end =
        row.gioKetThuc ??
        '—';

      return `${start} - ${end}`;
    },
  },

  {
    key: 'tenPhong',

    label: 'Phòng',

    value: (row) =>
      row.tenPhong ??
      row.idPhong ??
      '—',
  },

  {
    key: 'trangThai',

    label: 'Trạng thái',

    type: 'status',

    value: (row) =>
      row.trangThai === 'CANCELLED'
        ? 'Đã hủy'
        : 'Hiệu lực',
  },
];

const fields = [
  {
    key: 'idBacSi',

    label: 'Mã bác sĩ',

    type: 'number',

    min: 1,

    required: true,

    placeholder: 'VD: 1',

    colClass: 'col-md-4',
  },

  {
    key: 'ngayLamViec',

    label: 'Ngày làm việc',

    type: 'date',

    required: true,

    colClass: 'col-md-4',
  },

  {
    key: 'caLamViec',

    label: 'Ca làm việc',

    type: 'select',

    defaultValue: 'CUSTOM',

    options: [
      {
        value: 'MORNING',
        label: 'Ca sáng',
      },

      {
        value: 'AFTERNOON',
        label: 'Ca chiều',
      },

      {
        value: 'EVENING',
        label: 'Ca tối',
      },

      {
        value: 'CUSTOM',
        label: 'Tùy chỉnh',
      },
    ],

    colClass: 'col-md-4',
  },

  {
    key: 'gioBatDau',

    label: 'Giờ bắt đầu',

    type: 'time',

    required: true,

    colClass: 'col-md-3',
  },

  {
    key: 'gioKetThuc',

    label: 'Giờ kết thúc',

    type: 'time',

    required: true,

    colClass: 'col-md-3',
  },

  {
    key: 'idPhong',

    label: 'Mã phòng',

    placeholder: 'VD: P001',

    colClass: 'col-md-3',
  },

  {
    key: 'trangThai',

    label: 'Trạng thái',

    type: 'select',

    defaultValue: 'ACTIVE',

    options: [
      {
        value: 'ACTIVE',
        label: 'Hiệu lực',
      },

      {
        value: 'CANCELLED',
        label: 'Đã hủy',
      },
    ],

    colClass: 'col-md-3',
  },

  {
    key: 'ghiChu',

    label: 'Ghi chú',

    type: 'textarea',

    rows: 3,

    colClass: 'col-12',
  },
];

export default function QuanLyLichLamViec() {
  return (
    <AdminCrudPage
      title="Quản lý lịch làm việc"

      subtitle="Phân công lịch làm việc, thời gian và phòng khám cho bác sĩ."

      icon="fa-regular fa-calendar-days"

      columns={columns}

      fields={fields}

      loadItems={getWorkSchedules}

      createItem={createWorkSchedule}

      updateItem={updateWorkSchedule}

      deleteItem={deleteWorkSchedule}

      idKey="idLichLamViec"

      itemId={(row) =>
        row.idLichLamViec ??
        row.id
      }

      searchPlaceholder="Tìm mã lịch, bác sĩ, ngày làm việc, phòng..."

      addButtonText="Thêm lịch làm việc"

      formTitleCreate="Thêm lịch làm việc"

      formTitleEdit="Cập nhật lịch làm việc"
    />
  );
}