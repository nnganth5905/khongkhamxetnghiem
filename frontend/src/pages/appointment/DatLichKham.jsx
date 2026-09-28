import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

import {
  createExaminationAppointment,
  getAppointmentOptions,
  getApiErrorMessage,
  getTakenTimes,
} from '../../services/appointmentService';

const generateSlots = (start = '07:00', end = '16:00', stepMinutes = 60) => {
  const slots = [];
  const [startHour, startMin] = start.split(':').map(Number);
  const [endHour, endMin] = end.split(':').map(Number);
  const current = new Date();
  current.setHours(startHour, startMin, 0, 0);
  const endTime = new Date();
  endTime.setHours(endHour, endMin, 0, 0);

  while (current <= endTime) {
    const hh = String(current.getHours()).padStart(2, '0');
    const mm = String(current.getMinutes()).padStart(2, '0');
    slots.push(`${hh}:${mm}`);
    current.setMinutes(current.getMinutes() + stepMinutes);
  }
  return slots;
};

const SLOTS = generateSlots('07:00', '16:00', 60);
const CK_GENERAL = 'CK015';

const sameCK = (a, b) => {
  const A = String(a ?? '').toUpperCase();
  const B = String(b ?? '').toUpperCase();
  if (!A || !B) return false;
  if (A === B) return true;
  const na = A.replace(/\D/g, '');
  const nb = B.replace(/\D/g, '');
  return Boolean(na && nb && na === nb);
};

const getDoctorSpecialtyId = (doctor) =>
  doctor?.KhoaID ??
  doctor?.KhoaId ??
  doctor?.khoaID ??
  doctor?.khoaId ??
  doctor?.ChuyenKhoaID ??
  doctor?.ChuyenKhoaId ??
  doctor?.chuyenKhoaID ??
  doctor?.chuyenKhoaId ??
  doctor?.specialtyId ??
  doctor?.SpecialtyId ??
  '';

const getDoctorIdValue = (doctor) =>
  doctor?.IDBacSi ??
  doctor?.IdBacSi ??
  doctor?.idBacSi ??
  doctor?.idbacsi ??
  doctor?.id ??
  '';

const getLocalToday = () => {
  const now = new Date();
  const year = now.getFullYear();
  const month = String(now.getMonth() + 1).padStart(2, '0');
  const day = String(now.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
};

const getUserName = (user) => user?.FullName || user?.fullName || user?.HoTen || user?.hoTen || '';
const getUserEmail = (user) => user?.Email || user?.email || '';
const getUserPhone = (user) => user?.Phone || user?.phone || user?.SoDienThoai || user?.sodienthoai || user?.sdt || user?.phoneNumber || '';

export default function DatLichKham({
  departments: departmentsProp = null,
  allDoctors: allDoctorsProp = null,
  currentUser: currentUserProp = null,
}) {
  const navigate = useNavigate();
  const { user: authUser } = useAuth();
  const isGuest = !authUser;

  const [departments, setDepartments] = useState(departmentsProp || {});
  const [allDoctors, setAllDoctors] = useState(allDoctorsProp || []);
  
  const [currentUser, setCurrentUser] = useState(currentUserProp || authUser || null);

  const [selectedCK, setSelectedCK] = useState('');
  const [unknownCK, setUnknownCK] = useState(false);
  const [prevCK, setPrevCK] = useState('');
  const [selectedDoctor, setSelectedDoctor] = useState('');
  const [selectedDate, setSelectedDate] = useState('');
  const [selectedTime, setSelectedTime] = useState('');
  const [note, setNote] = useState('');
  
  const [forOther, setForOther] = useState(false);

  const [selfInfo, setSelfInfo] = useState({
    hoten: getUserName(currentUserProp || authUser),
    sdt: getUserPhone(currentUserProp || authUser),
    gioitinh: '',
    ngaysinh: '',
    email: getUserEmail(currentUserProp || authUser),
  });

  const [otherInfo, setOtherInfo] = useState({
    p_name: '',
    p_phone: '',
    p_gender: '',
    p_dob: '',
    p_email: '',
  });

  const [takenTimes, setTakenTimes] = useState(new Set());
  const [errMsg, setErrMsg] = useState('');
  const [successMsg, setSuccessMsg] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (authUser) {
      setCurrentUser(authUser);
      setSelfInfo((prev) => ({
        ...prev,
        hoten: prev.hoten || getUserName(authUser),
        email: prev.email || getUserEmail(authUser),
        sdt: prev.sdt || getUserPhone(authUser),
      }));
    }
  }, [authUser]);

  useEffect(() => {
    let active = true;
    const shouldLoad = !departmentsProp || !allDoctorsProp || !currentUserProp;

    if (!shouldLoad) return undefined;

    const loadOptions = async () => {
      try {
        const data = await getAppointmentOptions();
        if (!active) return;
        if (!departmentsProp) setDepartments(data?.departments || data?.chuyenKhoa || {});
        if (!allDoctorsProp) setAllDoctors(data?.doctors || data?.allDoctors || []);
        if (!currentUserProp && !authUser) setCurrentUser(data?.currentUser || data?.user || null);
      } catch (error) {
        if (active) setErrMsg(getApiErrorMessage(error, 'Không thể tải dữ liệu đặt lịch.'));
      }
    };

    loadOptions();
    return () => { active = false; };
  }, [departmentsProp, allDoctorsProp, currentUserProp, authUser]);

  useEffect(() => {
    if (!currentUser) return;
    setSelfInfo((prev) => ({
      ...prev,
      hoten: prev.hoten || getUserName(currentUser),
      email: prev.email || getUserEmail(currentUser),
      sdt: prev.sdt || getUserPhone(currentUser),
    }));
  }, [currentUser]);

  useEffect(() => {
    const params = new URLSearchParams(window.location.search);

    const prefillDoctor =
      params.get('idbs') ||
      params.get('doctorId') ||
      '';

    const prefillDate = params.get('date') || '';
    const prefillTime = params.get('time') || '';

    let prefillCK =
      params.get('khoa') ||
      params.get('ck') ||
      '';

    // Nếu URL có bác sĩ nhưng thiếu chuyên khoa,
    // tự tìm chuyên khoa của chính bác sĩ đó sau khi danh sách bác sĩ tải xong.
    if (!prefillCK && prefillDoctor && allDoctors.length > 0) {
      const doctor = allDoctors.find(
        (item) =>
          String(getDoctorIdValue(item)) === String(prefillDoctor)
      );

      if (doctor) {
        prefillCK = String(getDoctorSpecialtyId(doctor) || '');
      }
    }

    if (prefillCK) {
      setSelectedCK(prefillCK);
    } else if (!prefillDoctor && Object.keys(departments).length > 0) {
      // Chỉ dùng chuyên khoa mặc định khi không đi từ trang chọn bác sĩ.
      setSelectedCK((current) => current || Object.keys(departments)[0]);
    }

    if (prefillDoctor) {
      setSelectedDoctor(String(prefillDoctor));
    }

    if (prefillDate) {
      setSelectedDate(prefillDate);
    }

    if (prefillTime) {
      setSelectedTime(String(prefillTime).substring(0, 5));
    }
  }, [departments, allDoctors]);

  const filteredDoctors = useMemo(() => {
    // Khi người dùng chọn "Tôi chưa biết cần khám gì",
    // không khóa danh sách bác sĩ theo một chuyên khoa cố định.
    // Hiển thị toàn bộ bác sĩ để người dùng vẫn có thể chọn.
    if (unknownCK) {
      return allDoctors;
    }

    return allDoctors.filter((doctor) =>
      sameCK(getDoctorSpecialtyId(doctor), selectedCK)
    );
  }, [allDoctors, selectedCK, unknownCK]);

  const handleUnknownCKChange = (e) => {
    const checked = e.target.checked;
    setUnknownCK(checked);

    if (checked) {
      setPrevCK(selectedCK);

      // Ưu tiên Nội tổng quát nếu hệ thống có khoa này.
      // Tuy nhiên không ép danh sách bác sĩ phải thuộc CK015,
      // vì dữ liệu thực tế có thể không có bác sĩ gắn với CK015.
      setSelectedCK(CK_GENERAL);

      if (allDoctors.length > 0) {
        const firstDoctor = allDoctors[0];
        const firstDoctorId = getDoctorIdValue(firstDoctor);
        const firstDoctorCK = getDoctorSpecialtyId(firstDoctor);

        setSelectedDoctor(
          firstDoctorId !== null && firstDoctorId !== undefined
            ? String(firstDoctorId)
            : ''
        );

        // Nếu CK015 không tồn tại trong departments thì dùng chuyên khoa thật của bác sĩ.
        if (!Object.prototype.hasOwnProperty.call(departments, CK_GENERAL) && firstDoctorCK) {
          setSelectedCK(String(firstDoctorCK));
        }
      } else {
        setSelectedDoctor('');
      }
    } else {
      setSelectedCK(prevCK || Object.keys(departments)[0] || '');
      setSelectedDoctor('');
    }
  };

  useEffect(() => {
    if (!selectedDoctor || !selectedDate) {
      setTakenTimes(new Set());
      return;
    }
    let active = true;
    const loadTakenTimes = async () => {
      try {
        const data = await getTakenTimes('EXAMINATION', selectedDoctor, selectedDate);
        if (!active) return;
        const times = Array.isArray(data) ? data : data?.times || [];
        setTakenTimes(new Set(times.map((time) => String(time).substring(0, 5))));
      } catch {
        if (active) setTakenTimes(new Set());
      }
    };
    loadTakenTimes();
    return () => { active = false; };
  }, [selectedDoctor, selectedDate]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErrMsg('');
    setSuccessMsg('');

    if (!selectedDoctor || !selectedDate || !selectedTime || !selectedCK) {
      setErrMsg('Vui lòng chọn đầy đủ chuyên khoa, bác sĩ, ngày và giờ.');
      return;
    }

    if (takenTimes.has(selectedTime)) {
      setErrMsg('Khung giờ này vừa có người đặt. Vui lòng chọn giờ khác.');
      return;
    }

    const useManualInfo = isGuest || forOther;

    const finalName = useManualInfo
      ? otherInfo.p_name.trim()
      : selfInfo.hoten.trim();

    const finalPhone = useManualInfo
      ? otherInfo.p_phone.trim()
      : selfInfo.sdt.trim();

    const finalEmail = useManualInfo
      ? otherInfo.p_email.trim()
      : selfInfo.email.trim();

    const finalGender = useManualInfo
      ? otherInfo.p_gender
      : selfInfo.gioitinh;

    const finalDob = useManualInfo
      ? otherInfo.p_dob
      : selfInfo.ngaysinh;

    if (!finalName || !finalPhone || !finalEmail) {
      setErrMsg('Vui lòng nhập đầy đủ Họ tên, SĐT và Email của người khám.');
      return;
    }

    if (!/^[0-9+\-\s]{9,15}$/.test(finalPhone)) {
      setErrMsg('Số điện thoại không hợp lệ.');
      return;
    }

    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(finalEmail)) {
      setErrMsg('Vui lòng nhập email hợp lệ.');
      return;
    }

    const payload = {
      hoten: finalName,
      email: finalEmail,
      sodienthoai: finalPhone,
      gioitinh: finalGender,
      ngaysinh: finalDob,
      ngay: selectedDate,
      gio: selectedTime,
      idbacsi: selectedDoctor,
      idchuyenkhoa: selectedCK,
      idcoso: 'CS001',
      lydokham: note,
      ghichu: note,
    };

    try {
      setLoading(true);

      const result = await createExaminationAppointment(payload);

      const appointmentCode =
        result?.maDatLich ??
        result?.MaDatLich ??
        result?.appointmentCode ??
        result?.code ??
        result?.data?.maDatLich ??
        result?.data?.MaDatLich ??
        result?.data?.appointmentCode ??
        result?.data?.code ??
        '';

      if (!isGuest) {
        navigate('/lich-hen');
        return;
      }

      setSuccessMsg(
        appointmentCode
          ? `Đặt lịch thành công. Mã lịch của bạn: ${appointmentCode}. Hãy lưu mã này để tra cứu lịch hẹn.`
          : 'Đặt lịch thành công. Vui lòng lưu thông tin lịch hẹn để tra cứu khi cần.'
      );

      setNote('');
    } catch (error) {
      setErrMsg(getApiErrorMessage(error, 'Không thể lưu lịch.'));
    } finally {
      setLoading(false);
    }
  };

  const todayStr = getLocalToday();

  return (
    <div className="container py-4" style={{ maxWidth: '780px' }}>
      <h3 className="mb-3 fw-bold">ĐẶT LỊCH KHÁM</h3>

      {errMsg && <div className="alert alert-danger">{errMsg}</div>}
      {successMsg && (
        <div className="alert alert-success">
          <div className="fw-semibold mb-1">Đặt lịch thành công</div>
          <div>{successMsg}</div>
        </div>
      )}

      <form onSubmit={handleSubmit} className="row g-3" noValidate>
        {/* Chuyên khoa */}
        <div className="col-md-6">
          <label className="form-label d-flex align-items-center justify-content-between">
            <span>Chuyên khoa</span>
            <span className="form-check ms-2">
              <input
                type="checkbox"
                id="unknownChk"
                className="form-check-input"
                checked={unknownCK}
                onChange={handleUnknownCKChange}
              />
              <label className="form-check-label" htmlFor="unknownChk" style={{ cursor: 'pointer' }}>
                Tôi chưa biết cần khám gì
              </label>
            </span>
          </label>
          <select
            className="form-select"
            disabled={unknownCK}
            value={selectedCK}
            onChange={(e) => {
              setSelectedCK(e.target.value);
              setSelectedDoctor('');
            }}
          >
            {Object.entries(departments).map(([id, name]) => (
              <option key={id} value={id}>{name}</option>
            ))}
          </select>
        </div>

        {/* Ngày */}
        <div className="col-md-6">
          <label className="form-label">Ngày khám *</label>
          <input
            type="date"
            className="form-control"
            required
            min={todayStr}
            value={selectedDate}
            onChange={(e) => setSelectedDate(e.target.value)}
          />
        </div>

        {/* Giờ */}
        <div className="col-md-6">
          <label className="form-label">Giờ *</label>
          <select
            className="form-select"
            required
            value={selectedTime}
            onChange={(e) => setSelectedTime(e.target.value)}
          >
            <option value="">-- Chọn giờ --</option>
            {SLOTS.map((slot) => {
              const isTaken = takenTimes.has(slot);
              return (
                <option key={slot} value={slot} disabled={isTaken}>
                  {slot} {isTaken ? '(đã kín)' : ''}
                </option>
              );
            })}
          </select>
        </div>

        {/* Bác sĩ */}
        <div className="col-md-6">
          <label className="form-label">Bác sĩ *</label>
          <select
            className="form-select"
            required
            value={selectedDoctor}
            onChange={(e) => {
              const doctorId = e.target.value;
              setSelectedDoctor(doctorId);

              // Khi chưa biết khám gì, chuyên khoa sẽ đi theo bác sĩ được chọn
              // nếu hệ thống không có khoa Nội tổng quát cố định.
              if (unknownCK && doctorId) {
                const doctor = allDoctors.find(
                  (item) =>
                    String(getDoctorIdValue(item)) === String(doctorId)
                );

                const doctorCK = getDoctorSpecialtyId(doctor);

                if (
                  !Object.prototype.hasOwnProperty.call(departments, CK_GENERAL) &&
                  doctorCK
                ) {
                  setSelectedCK(String(doctorCK));
                }
              }
            }}
          >
            <option value="">-- Chọn --</option>
            {filteredDoctors.length === 0 && (
              <option value="" disabled>
                Chưa có bác sĩ phù hợp
              </option>
            )}
            {filteredDoctors.map((doctor) => {
              const id = doctor.IDBacSi ?? doctor.id;
              const name = doctor.TenBacSi ?? doctor.doctorName ?? doctor.name;
              return (
                <option key={id} value={id}>
                  {name}
                </option>
              );
            })}
          </select>
        </div>

        {/* Trạng thái người đặt */}
        <div className="col-12 mt-4">
          {isGuest ? (
            <div className="alert alert-info mb-0">
              Bạn đang đặt lịch khi chưa đăng nhập. Vui lòng nhập thông tin người khám bên dưới.
            </div>
          ) : (
            <div className="form-check">
              <input
                type="checkbox"
                id="forOther"
                className="form-check-input"
                checked={forOther}
                onChange={(e) => setForOther(e.target.checked)}
              />
              <label className="form-check-label fw-bold" htmlFor="forOther">
                Đặt cho người khác
              </label>
            </div>
          )}
        </div>

        {/* KHỐI NHẬP THÔNG TIN: khách chưa đăng nhập hoặc đặt cho người khác */}
        {(forOther || isGuest) && (
          <div className="col-12">
            <div className="row g-3 p-3 border rounded bg-light">
              <div className="col-md-6">
                <label className="form-label">Họ tên *</label>
                <input
                  type="text"
                  className="form-control"
                  value={otherInfo.p_name}
                  onChange={(e) => setOtherInfo({ ...otherInfo, p_name: e.target.value })}
                />
              </div>

              <div className="col-md-3">
                <label className="form-label">SĐT *</label>
                <input
                  type="text"
                  className="form-control"
                  value={otherInfo.p_phone}
                  onChange={(e) => setOtherInfo({ ...otherInfo, p_phone: e.target.value })}
                />
              </div>

              <div className="col-md-3">
                <label className="form-label">Giới tính</label>
                <select
                  className="form-select"
                  value={otherInfo.p_gender}
                  onChange={(e) => setOtherInfo({ ...otherInfo, p_gender: e.target.value })}
                >
                  <option value="">--</option>
                  <option value="nam">Nam</option>
                  <option value="nu">Nữ</option>
                  <option value="khac">Khác</option>
                </select>
              </div>

              <div className="col-md-4">
                <label className="form-label">Ngày sinh</label>
                <input
                  type="date"
                  className="form-control"
                  value={otherInfo.p_dob}
                  onChange={(e) => setOtherInfo({ ...otherInfo, p_dob: e.target.value })}
                />
              </div>

              <div className="col-md-8">
                <label className="form-label">Email nhận thông tin *</label>
                <input
                  type="email"
                  className="form-control"
                  value={otherInfo.p_email}
                  onChange={(e) => setOtherInfo({ ...otherInfo, p_email: e.target.value })}
                />
              </div>
            </div>
          </div>
        )}

        {/* Ghi chú */}
        <div className="col-12 mt-3">
          <label className="form-label">Ghi chú</label>
          <textarea
            rows="2"
            className="form-control"
            placeholder="Yêu cầu thêm (nếu có)"
            value={note}
            onChange={(e) => setNote(e.target.value)}
          />
        </div>

        <div className="col-12 mt-4">
          <button
            type="submit"
            className="btn btn-primary px-4 py-2"
            disabled={loading}
            style={{
              backgroundColor: 'var(--primary, #0b63e5)',
              borderColor: 'var(--primary, #0b63e5)',
            }}
          >
            {loading ? 'Đang xử lý...' : 'Xác nhận'}
          </button>
        </div>
      </form>
    </div>
  );
}