import api, { unwrap } from './api';

const arr = (value) => Array.isArray(value) ? value : [];
const statusToYesNo = (v) => {
  const s = String(v ?? '').toLowerCase();
  return ['active','available','yes','1','true'].includes(s) ? 'yes' : ['inactive','no','0','false'].includes(s) ? 'no' : (v ?? 'yes');
};

const mapCustomer = (x) => ({
  ...x,
  idKhachHang: x.id ?? x.idKhachHang,
  tenKhachHang: x.name ?? x.tenKhachHang,
  soDienThoai: x.phone ?? x.soDienThoai,
  gioiTinh: x.gender ?? x.gioiTinh,
  ngaySinh: x.birthday ?? x.ngaySinh,
  diaChi: x.address ?? x.diaChi,
});
const customerPayload = (x) => ({
  name: x.tenKhachHang ?? x.name ?? '', email: x.email || null,
  phone: x.soDienThoai ?? x.phone ?? null, gender: x.gioiTinh ?? x.gender ?? null,
  birthday: x.ngaySinh ?? x.birthday ?? null, address: x.diaChi ?? x.address ?? null,
  cccd: x.cccd || null, status: statusToYesNo(x.status)
});

const mapEmployee = (x) => ({
  ...x, idNhanVien: x.id ?? x.idNhanVien, hoTen: x.name ?? x.hoTen,
  vaiTro: x.position ?? x.vaiTro, soDienThoai: x.phone ?? x.soDienThoai,
  idCoSo: x.facilityId ?? x.idCoSo, trangThai: x.status ?? x.trangThai
});
const employeePayload = (x) => ({
  name: x.hoTen ?? x.name ?? '', position: x.vaiTro ?? x.position ?? 'STAFF',
  phone: x.soDienThoai ?? x.phone ?? null, email: x.email || null,
  facilityId: x.idCoSo ?? x.facilityId ?? null,
  status: statusToYesNo(x.trangThai ?? x.status)
});

const mapDoctor = (x) => ({
  ...x, idBacSi: x.id ?? x.idBacSi, hoTen: x.name ?? x.hoTen,
  tenBacSi: x.name ?? x.tenBacSi, hocVi: x.degree ?? x.hocVi,
  idChuyenKhoa: x.specialtyId ?? x.idChuyenKhoa,
  tenChuyenKhoa: x.specialty ?? x.tenChuyenKhoa,
  idCoSo: x.facilityId ?? x.idCoSo, gioiThieu: x.description ?? x.gioiThieu,
  trangThai: x.status ?? x.trangThai
});
const doctorPayload = (x) => ({
  name: x.hoTen ?? x.tenBacSi ?? x.name ?? '', degree: x.hocVi ?? x.degree ?? null,
  title: x.chucDanh ?? x.title ?? null, specialtyId: x.idChuyenKhoa ?? x.specialtyId ?? '',
  facilityId: x.idCoSo ?? x.facilityId ?? null,
  description: x.gioiThieu ?? x.description ?? null, image: x.image ?? null,
  rating: x.rating ?? null, experience: x.experience ?? null,
  status: String(x.trangThai ?? x.status ?? 'active').toLowerCase() === 'inactive' ? 'inactive' : 'active',
  employeeId: x.employeeId ?? null
});

const mapTechnician = (x) => ({
  ...x, idNhanVien: x.id ?? x.idNhanVien, maKTV: x.id ?? x.maKTV,
  hoTen: x.name ?? x.hoTen, soDienThoai: x.phone ?? x.soDienThoai,
  idCoSo: x.facilityId ?? x.idCoSo, trangThai: x.status ?? x.trangThai
});

const mapTest = (x) => ({ ...x, idXetNghiem: x.id ?? x.idXetNghiem, maXetNghiem: x.id ?? x.maXetNghiem,
  tenXetNghiem: x.name ?? x.tenXetNghiem, idLoaiXetNghiem: x.specialtyId ?? x.idLoaiXetNghiem,
  loaiMau: x.sampleType ?? x.loaiMau, gia: x.price ?? x.gia, moTa: x.description ?? x.moTa,
  trangThai: x.status ?? x.trangThai });
const testPayload = (x) => ({ name: x.tenXetNghiem ?? x.name ?? '', specialtyId: x.idLoaiXetNghiem ?? x.specialtyId ?? '',
  type: x.loai ?? x.type ?? null, description: x.moTa ?? x.description ?? null,
  price: Number(x.gia ?? x.price ?? 0), sampleType: x.loaiMau ?? x.sampleType ?? null,
  expectedMinutes: x.expectedMinutes ? Number(x.expectedMinutes) : null,
  status: statusToYesNo(x.trangThai ?? x.status) });

const mapSpecialty = (x) => ({ ...x, idChuyenKhoa: x.id ?? x.idChuyenKhoa, maChuyenKhoa: x.id ?? x.maChuyenKhoa,
  tenChuyenKhoa: x.name ?? x.tenChuyenKhoa, moTa: x.description ?? x.moTa, trangThai: x.status ?? x.trangThai });
const specialtyPayload = (x) => ({ name: x.tenChuyenKhoa ?? x.name ?? '', description: x.moTa ?? x.description ?? null,
  status: statusToYesNo(x.trangThai ?? x.status) });

const mapRoom = (x) => ({ ...x, idPhong: x.id ?? x.idPhong, maPhong: x.id ?? x.maPhong,
  tenPhong: x.name ?? x.tenPhong, idCoSo: x.facilityId ?? x.idCoSo, idChuyenKhoa: x.specialtyId ?? x.idChuyenKhoa,
  loaiPhong: x.type ?? x.loaiPhong, tang: x.floor ?? x.tang, trangThai: x.status ?? x.trangThai });
const roomPayload = (x) => ({ name: x.tenPhong ?? x.name ?? '', facilityId: x.idCoSo ?? x.facilityId ?? '',
  specialtyId: x.idChuyenKhoa ?? x.specialtyId ?? null, type: x.loaiPhong ?? x.type ?? 'kham',
  floor: x.tang ?? x.floor ?? null, status: x.trangThai ?? x.status ?? 'active' });

const mapSchedule = (x) => ({ ...x, idLichLamViec: x.id ?? x.idLichLamViec, maLich: x.id ?? x.maLich,
  idNhanSu: x.doctorId ?? x.idNhanSu, tenNhanSu: x.doctorName ?? x.tenNhanSu,
  ngayLamViec: x.date ?? x.ngayLamViec, caLamViec: x.shift ?? x.caLamViec,
  gioBatDau: x.start ?? x.gioBatDau, gioKetThuc: x.end ?? x.gioKetThuc,
  idPhong: x.roomId ?? x.idPhong, trangThai: x.status ?? x.trangThai, ghiChu: x.note ?? x.ghiChu });
const schedulePayload = (x) => ({ doctorId: Number(x.idNhanSu ?? x.doctorId ?? 0), roomId: x.idPhong ?? x.roomId ?? null,
  date: x.ngayLamViec ?? x.date, shift: x.caLamViec ?? x.shift ?? 'sang', start: x.gioBatDau ?? x.start,
  end: x.gioKetThuc ?? x.end, status: x.trangThai ?? x.status ?? 'active', note: x.ghiChu ?? x.note ?? null });

export const getCustomers = async (params = {}) => arr(unwrap(await api.get('/admin/customers', { params }))).map(mapCustomer);
export const createCustomer = async (p) => unwrap(await api.post('/admin/customers', customerPayload(p)));
export const updateCustomer = async (id,p) => unwrap(await api.put(`/admin/customers/${id}`, customerPayload(p)));
export const deleteCustomer = async (id) => unwrap(await api.delete(`/admin/customers/${id}`));

export const getEmployees = async (params = {}) => arr(unwrap(await api.get('/admin/employees', { params }))).map(mapEmployee);
export const createEmployee = async (p) => unwrap(await api.post('/admin/employees', employeePayload(p)));
export const updateEmployee = async (id,p) => unwrap(await api.put(`/admin/employees/${id}`, employeePayload(p)));
export const deleteEmployee = async (id) => unwrap(await api.delete(`/admin/employees/${id}`));

export const getDoctors = async (params = {}) => arr(unwrap(await api.get('/admin/doctors', { params }))).map(mapDoctor);
export const createDoctor = async (p) => unwrap(await api.post('/admin/doctors', doctorPayload(p)));
export const updateDoctor = async (id,p) => unwrap(await api.put(`/admin/doctors/${id}`, doctorPayload(p)));
export const deleteDoctor = async (id) => unwrap(await api.delete(`/admin/doctors/${id}`));

export const getTechnicians = async (params = {}) => arr(unwrap(await api.get('/admin/technicians', { params }))).map(mapTechnician);
export const createTechnician = async (p) => unwrap(await api.post('/admin/technicians', employeePayload({ ...p, vaiTro: 'ktv' })));
export const updateTechnician = async (id,p) => unwrap(await api.put(`/admin/technicians/${id}`, employeePayload({ ...p, vaiTro: 'ktv' })));
export const deleteTechnician = async (id) => unwrap(await api.delete(`/admin/technicians/${id}`));

export const getAdminTests = async (params = {}) => arr(unwrap(await api.get('/admin/tests', { params }))).map(mapTest);
export const createAdminTest = async (p) => unwrap(await api.post('/admin/tests', testPayload(p)));
export const updateAdminTest = async (id,p) => unwrap(await api.put(`/admin/tests/${id}`, testPayload(p)));
export const deleteAdminTest = async (id) => unwrap(await api.delete(`/admin/tests/${id}`));

export const getSpecialties = async (params = {}) => arr(unwrap(await api.get('/admin/specialties', { params }))).map(mapSpecialty);
export const createSpecialty = async (p) => unwrap(await api.post('/admin/specialties', specialtyPayload(p)));
export const updateSpecialty = async (id,p) => unwrap(await api.put(`/admin/specialties/${id}`, specialtyPayload(p)));
export const deleteSpecialty = async (id) => unwrap(await api.delete(`/admin/specialties/${id}`));

export const getRooms = async (params = {}) => arr(unwrap(await api.get('/admin/rooms', { params }))).map(mapRoom);
export const createRoom = async (p) => unwrap(await api.post('/admin/rooms', roomPayload(p)));
export const updateRoom = async (id,p) => unwrap(await api.put(`/admin/rooms/${id}`, roomPayload(p)));
export const deleteRoom = async (id) => unwrap(await api.delete(`/admin/rooms/${id}`));

export const getWorkSchedules = async (params = {}) => arr(unwrap(await api.get('/admin/work-schedules', { params }))).map(mapSchedule);
export const createWorkSchedule = async (p) => unwrap(await api.post('/admin/work-schedules', schedulePayload(p)));
export const updateWorkSchedule = async (id,p) => unwrap(await api.put(`/admin/work-schedules/${id}`, schedulePayload(p)));
export const deleteWorkSchedule = async (id) => unwrap(await api.delete(`/admin/work-schedules/${id}`));

export const getDashboardStats = async (params = {}) => unwrap(await api.get('/admin/dashboard', { params }));
