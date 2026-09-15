import React from 'react';

import AdminCrudPage from '../../components/admin/AdminCrudPage';

import {
  getWorkSchedules,
  createWorkSchedule,
  updateWorkSchedule,
  deleteWorkSchedule,
} from '../../services/adminService';

const columns = [
  {
    label: 'Mã lịch',
    value: (row) =>
      row.maLich ??
      row.MaLich ??
      row.idLichLamViec ??
      row.IDLichLamViec,
  },
  {
    label: 'Nhân sự',
    value: (row) =>
      row.tenNhanSu ??
      row.TenNhanSu ??
      row.tenBacSi ??
      row.TenBacSi ??
      row.idNhanVien ??
      row.IDNhanVien ??
      '—',
  },
  {
    label: 'Ngày làm việc',
    value: (row) =>
      row.ngayLamViec ??
      row.NgayLamViec ??
      '—',
  },
  {
    label: 'Ca',
    value: (row) =>
      row.caLamViec ??
      row.CaLamViec ??
      '—',
  },
  {
    label: 'Thời gian',
    value: (row) => {
      const start =
        row.gioBatDau ??
        row.GioBatDau ??
        '—';

      const end =
        row.gioKetThuc ??
        row.GioKetThuc ??
        '—';

      return `${start} - ${end}`;
    },
  },
  {
    label: 'Phòng',
    value: (row) =>
      row.tenPhong ??
      row.TenPhong ??
      row.idPhong ??
      row.IDPhong ??
      '—',
  },
  {
    label: 'Trạng thái',
    type: 'status',
    value: (row) =>
      row.trangThai ??
      row.TrangThai ??
      row.status ??
      'ACTIVE',
  },
];

const fields = [
  {
    key: 'loaiNhanSu',
    label: 'Loại nhân sự',
    type: 'select',
    required: true,
    options: [
      {
        value: 'DOCTOR',
        label: 'Bác sĩ',
      },
      {
        value: 'TECHNICIAN',
        label: 'Kỹ thuật viên',
      },
      {
        value: 'RECEPTIONIST',
        label: 'Lễ tân',
      },
    ],
    colClass: 'col-md-3',
  },
  {
    key: 'idNhanSu',
    label: 'Mã nhân sự',
    required: true,
    colClass: 'col-md-3',
  },
  {
    key: 'ngayLamViec',
    label: 'Ngày làm việc',
    type: 'date',
    required: true,
    colClass: 'col-md-3',
  },
  {
    key: 'caLamViec',
    label: 'Ca làm việc',
    type: 'select',
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
        value: 'FULL_DAY',
        label: 'Cả ngày',
      },
    ],
    colClass: 'col-md-3',
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
    colClass: 'col-12',
  },
];

export default function QuanLyLichLamViec() {
  return (
    <AdminCrudPage
      title="Quản lý lịch làm việc"
      subtitle="Phân công lịch làm việc cho bác sĩ, kỹ thuật viên và lễ tân."
      icon="fa-regular fa-calendar-days"
      columns={columns}
      fields={fields}
      loadItems={getWorkSchedules}
      createItem={createWorkSchedule}
      updateItem={updateWorkSchedule}
      deleteItem={deleteWorkSchedule}
      itemId={(row) =>
        row.id ??
        row.idLichLamViec ??
        row.IDLichLamViec
      }
      searchPlaceholder="Tìm mã lịch, nhân sự, ngày làm việc, phòng..."
      addButtonText="Thêm lịch làm việc"
      formTitleCreate="Thêm lịch làm việc"
      formTitleEdit="Cập nhật lịch làm việc"
    />
  );
}