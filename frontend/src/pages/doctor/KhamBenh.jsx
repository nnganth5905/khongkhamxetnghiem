// src/pages/doctor/KhamBenh.jsx
import React, { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { getApiErrorMessage } from '../../services/api';
import visitService from '../../services/visitService';

export default function KhamBenh() {
  const [searchParams] = useSearchParams();
  const luotKhamId = searchParams.get('luotKhamId'); // Lấy ID từ URL
  const navigate = useNavigate();

  const [patientInfo, setPatientInfo] = useState({});
  const [idKham, setIdKham] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' });

  // Map đúng với các trường trong cơ sở dữ liệu
  const [medicalRecord, setMedicalRecord] = useState({
    trieuChung: '',
    tienSuBenh: '',
    chanDoan: '',
    ketLuan: '',
    huongDieuTri: '',
  });

  useEffect(() => {
    if (!luotKhamId) {
      setMessage({ type: 'danger', text: 'Không tìm thấy mã lượt khám.' });
      setLoading(false);
      return;
    }

    const fetchExaminationData = async () => {
      try {
        setLoading(true);
        // Gọi API lấy dữ liệu khám bệnh
        const data = await visitService.getExamination(luotKhamId);
        
        const gender = data.gioiTinh ?? data.GioiTinh;

setPatientInfo({
  TenKhachHang:
    data.tenKhachHang ??
    data.TenKhachHang ??
    '',

  GioiTinh:
    gender === 'nam'
      ? 'Nam'
      : gender === 'nu'
      ? 'Nữ'
      : gender
      ? 'Khác'
      : '',

  NgaySinh:
    data.ngaySinh ??
    data.NgaySinh ??
    '',

  SoDienThoai:
    data.soDienThoai ??
    data.SoDienThoai ??
    ''
});

setIdKham(data.idKham ?? data.IDKham ?? null);

setMedicalRecord({
  trieuChung: data.trieuChung ?? data.TrieuChung ?? '',
  tienSuBenh: data.tienSuBenh ?? data.TienSuBenh ?? '',
  chanDoan: data.chanDoan ?? data.ChanDoan ?? '',
  ketLuan: data.ketLuan ?? data.KetLuan ?? '',
  huongDieuTri: data.huongDieuTri ?? data.HuongDieuTri ?? '',
});
        
        
      } catch (err) {
        setMessage({ type: 'danger', text: getApiErrorMessage(err, 'Không thể tải thông tin bệnh án.') });
      } finally {
        setLoading(false);
      }
    };

    fetchExaminationData();
  }, [luotKhamId]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setMedicalRecord((prev) => ({ ...prev, [name]: value }));
  };

  const handleSave = async (e) => {
    e.preventDefault();
    setSaving(true);
    setMessage({ type: '', text: '' });

    try {
      // Truyền idKham, dữ liệu và luotKhamId vào hàm save
      await visitService.saveExamination(idKham, medicalRecord, luotKhamId); 
      setMessage({ type: 'success', text: 'Lưu hồ sơ bệnh án thành công!' });
    } catch (err) {
      setMessage({ type: 'danger', text: getApiErrorMessage(err, 'Có lỗi xảy ra khi lưu kết quả khám.') });
    } finally {
      setSaving(false);
    }
  };

  const handleComplete = async () => {
    if (!window.confirm('Xác nhận hoàn tất phiên khám bệnh này?')) return;
    try {
      // Lưu lại lần cuối trước khi hoàn tất
      await visitService.saveExamination(idKham, medicalRecord, luotKhamId);
      await visitService.completeExamination(luotKhamId);
      alert('Đã hoàn tất khám bệnh!');
      navigate('/doctor/danh-sach-cho');
    } catch (err) {
      setMessage({ type: 'danger', text: getApiErrorMessage(err, 'Lỗi khi hoàn tất khám.') });
    }
  };

  const handleOrderTest = async () => {
  if (!luotKhamId) {
    setMessage({
      type: 'danger',
      text: 'Không tìm thấy mã lượt khám.',
    });
    return;
  }

  try {
    // Nếu đã có hồ sơ khám thì lưu trước.
    if (idKham) {
      await visitService.saveExamination(
        idKham,
        medicalRecord,
        luotKhamId
      );
    }

    let url =
      `/doctor/chi-dinh-xet-nghiem?luotKhamId=${encodeURIComponent(
        luotKhamId
      )}`;

    if (idKham) {
      url += `&idKham=${encodeURIComponent(idKham)}`;
    }

    navigate(url);
  } catch (err) {
    setMessage({
      type: 'danger',
      text: getApiErrorMessage(
        err,
        'Không thể chuyển sang chỉ định xét nghiệm.'
      ),
    });
  }
};

  return (
    <div className="container-fluid py-4" style={{ maxWidth: '900px', minHeight: '100vh', backgroundColor: '#f8f9fa' }}>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2 className="fw-bold mb-1" style={{ color: 'var(--primary, #0d6efd)' }}>Khám bệnh & Kê đơn</h2>
          <p className="text-secondary mb-0">Hồ sơ thăm khám lâm sàng của bệnh nhân</p>
        </div>
        <button type="button" className="btn btn-outline-secondary bg-white" onClick={() => navigate('/doctor/danh-sach-cho')}>
          <i className="fa-solid fa-arrow-left me-2"></i> Quay lại
        </button>
      </div>

      {message.text && (
        <div className={`alert alert-${message.type} alert-dismissible fade show shadow-sm`} role="alert">
          {message.text}
          <button type="button" className="btn-close" onClick={() => setMessage({ type: '', text: '' })}></button>
        </div>
      )}

      {loading ? (
        <div className="text-center py-5"><div className="spinner-border text-primary"></div></div>
      ) : (
        <div className="row g-4">
          {/* Thông tin bệnh nhân */}
          <div className="col-12">
            <div className="card border-0 shadow-sm rounded-4 bg-white">
              <div className="card-body p-4">
                <h5 className="card-title fw-bold text-primary mb-3"><i className="fa-regular fa-address-card me-2"></i>Thông tin bệnh nhân</h5>
                <div className="row g-3">
                  <div className="col-md-6">
                    <span className="text-muted d-block small">Họ và tên</span>
                    <strong className="text-danger fs-5">{patientInfo.TenKhachHang || '---'}</strong>
                  </div>
                  <div className="col-md-6">
                    <span className="text-muted d-block small">Số điện thoại</span>
                    <strong>{patientInfo.SoDienThoai || '---'}</strong>
                  </div>
                  <div className="col-md-6">
                    <span className="text-muted d-block small">Giới tính</span>
                    <strong>{patientInfo.GioiTinh || '---'}</strong>
                  </div>
                  <div className="col-md-6">
                    <span className="text-muted d-block small">Ngày sinh</span>
                    <strong>{patientInfo.NgaySinh || '---'}</strong>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Form nhập liệu */}
          <div className="col-12">
            <form className="card border-0 shadow-sm rounded-4 p-4 bg-white" onSubmit={handleSave}>
              <div className="mb-3">
                <label className="form-label fw-semibold">Lý do khám / Triệu chứng lâm sàng <span className="text-danger">*</span></label>
                <textarea name="trieuChung" rows="3" className="form-control" placeholder="Ghi nhận triệu chứng..." value={medicalRecord.trieuChung} onChange={handleChange} required />
              </div>

              <div className="mb-3">
                <label className="form-label fw-semibold">Tiền sử bệnh</label>
                <textarea name="tienSuBenh" rows="2" className="form-control" placeholder="Bệnh lý nền, dị ứng..." value={medicalRecord.tienSuBenh} onChange={handleChange} />
              </div>

              <div className="mb-3">
                <label className="form-label fw-semibold">Chẩn đoán xác định</label>
                <input type="text" name="chanDoan" className="form-control" placeholder="Mã ICD / Chẩn đoán bệnh học..." value={medicalRecord.chanDoan} onChange={handleChange} />
              </div>

              <div className="mb-3">
                <label className="form-label fw-semibold">Kết luận</label>
                <textarea name="ketLuan" rows="2" className="form-control" placeholder="Tóm tắt tình trạng..." value={medicalRecord.ketLuan} onChange={handleChange} />
              </div>

              <div className="mb-4">
                <label className="form-label fw-semibold">Hướng điều trị / Lời dặn</label>
                <textarea name="huongDieuTri" rows="3" className="form-control" placeholder="Chế độ sinh hoạt, đơn thuốc, tái khám..." value={medicalRecord.huongDieuTri} onChange={handleChange} />
              </div>

              <hr className="my-4"/>

              <div className="d-flex justify-content-between align-items-center flex-wrap gap-3">
                <button type="submit" className="btn btn-outline-primary px-4 fw-medium" disabled={saving}>
                  {saving ? <><span className="spinner-border spinner-border-sm me-2"></span>Đang lưu...</> : <><i className="fa-regular fa-floppy-disk me-2"></i>Lưu thông tin</>}
                </button>

                <div className="d-flex gap-2">
                  <button
                    type="button"
                    className="btn btn-warning px-4 fw-medium text-dark shadow-sm"
                    onClick={handleOrderTest}
                    disabled={saving || !luotKhamId}
                    title={!luotKhamId || !idKham ? 'Hãy mở ca bệnh từ Danh sách chờ và lưu hồ sơ trước.' : ''}
                  >
                    <i className="fa-solid fa-vial-circle-check me-2"></i> Chỉ định xét nghiệm
                  </button>
                  <button type="button" className="btn btn-success px-4 fw-medium shadow-sm" onClick={handleComplete}>
                    <i className="fa-solid fa-check-double me-2"></i> Hoàn tất khám
                  </button>
                </div>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}