import React from 'react';
import AdminCrudPage from '../../components/admin/AdminCrudPage';
import {
  getCustomers,
  createCustomer,
  updateCustomer,
  deleteCustomer,
} from '../../services/adminService';

const columns = [
  {
    key: 'idKhachHang',
    label: 'Mã KH',
    value: (row) => row.idKhachHang,
  },
  {
    key: 'tenKhachHang',
    label: 'Họ và tên',
    value: (row) => row.tenKhachHang,
  },
  {
    key: 'soDienThoai',
    label: 'Số điện thoại',
    value: (row) => row.soDienThoai,
  },
  {
    key: 'cccd',
    label: 'CCCD/CMND',
    value: (row) => row.cccd,
  },
  {
    key: 'email',
    label: 'Email',
    value: (row) => row.email,
  },
  {
    key: 'gioiTinh',
    label: 'Giới tính',
    value: (row) => row.gioiTinh === 'nam' ? 'Nam' : row.gioiTinh === 'nu' ? 'Nữ' : row.gioiTinh === 'khac' ? 'Khác' : '—',
  },
  {
    key: 'status',
    label: 'Trạng thái',
    type: 'status',
    value: (row) => row.status === 'yes' ? 'Hoạt động' : 'Đã khóa',
  },
];

const fields = [
  {
    key: 'tenKhachHang',
    label: 'Họ và tên',
    required: true,
    colClass: 'col-md-6',
  },
  {
    key: 'soDienThoai',
    label: 'Số điện thoại',
    required: true,
    colClass: 'col-md-3',
  },
  {
    key: 'ngaySinh',
    label: 'Ngày sinh',
    type: 'date',
    colClass: 'col-md-3',
  },
  {
    key: 'cccd',
    label: 'CCCD/CMND',
    colClass: 'col-md-4',
  },
  {
    key: 'email',
    label: 'Email',
    type: 'email',
    colClass: 'col-md-4',
  },
  {
    key: 'gioiTinh',
    label: 'Giới tính',
    type: 'select',
    options: [
      { value: 'nam', label: 'Nam' },
      { value: 'nu', label: 'Nữ' },
      { value: 'khac', label: 'Khác' },
    ],
    colClass: 'col-md-4',
  },
  {
    key: 'diaChi',
    label: 'Địa chỉ',
    colClass: 'col-md-8',
  },
  {
    key: 'status',
    label: 'Trạng thái',
    type: 'select',
    defaultValue: 'yes',
    options: [
      { value: 'yes', label: 'Hoạt động' },
      { value: 'no', label: 'Ngừng hoạt động' },
    ],
    colClass: 'col-md-4',
  },
];

export default function QuanLyKhachHang() {
  return (
    <AdminCrudPage
      title="Quản lý khách hàng"
      subtitle="Quản lý hồ sơ khách hàng sử dụng dịch vụ tại Bio Medic Center."
      icon="fa-solid fa-users"
      columns={columns}
      fields={fields}
      loadItems={getCustomers}
      createItem={createCustomer}
      updateItem={updateCustomer}
      deleteItem={deleteCustomer}
      idKey="idKhachHang"
      itemId={(row) => row.idKhachHang}
      searchPlaceholder="Tìm theo mã khách hàng, họ tên, số điện thoại, CCCD..."
      addButtonText="Thêm khách hàng"
      formTitleCreate="Thêm khách hàng"
      formTitleEdit="Cập nhật khách hàng"
    />
  );
}