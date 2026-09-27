import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { getApiErrorMessage, getAppointmentDetail } from '../../services/appointmentService';

// --- HÀM FORMAT THỜI GIAN ---
const formatDate = (dateStr) => {
  if (!dateStr) return '—';
  const cleanDate = String(dateStr).substring(0, 10);
  const parts = cleanDate.split('-');
  if (parts.length === 3) return `${parts[2]}/${parts[1]}/${parts[0]}`;
  
  const d = new Date(dateStr);
  if (Number.isNaN(d.getTime())) return dateStr;
  return `${String(d.getDate()).padStart(2, '0')}/${String(d.getMonth() + 1).padStart(2, '0')}/${d.getFullYear()}`;
};

const formatDateTime = (dateTimeStr) => {
  if (!dateTimeStr) return '—';
  const d = new Date(dateTimeStr);
  if (Number.isNaN(d.getTime())) return dateTimeStr;
  
  const time = d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
  const date = `${String(d.getDate()).padStart(2, '0')}/${String(d.getMonth() + 1).padStart(2, '0')}/${d.getFullYear()}`;
  return `${time} - ${date}`;
};

// ĐÃ CẬP NHẬT: Nhận thêm biến type
const renderStatusBadge = (status, type) => {
  const value = String(status || '').toLowerCase();
  
  if (['cancelled', 'huy'].includes(value)) {
    return <span className="badge bg-danger px-3 py-2 rounded-pill">Đã hủy lịch hẹn</span>;
  }
  if (['no_show', 'bo_luot'].includes(value)) {
    return <span className="badge bg-dark px-3 py-2 rounded-pill">Bỏ lượt</span>;
  }
  if (['rescheduled', 'da_doi_lich'].includes(value)) {
    return <span className="badge bg-warning text-dark px-3 py-2 rounded-pill">Đã đổi lịch hẹn</span>;
  }

  let label = status;
  let bgClass = "bg-primary";
  const isExam = type === 'EXAMINATION';
  
  switch (value) {
    case 'pending': 
      label = 'Đã đặt lịch'; bgClass = 'bg-info text-dark'; break;
    case 'confirmed': 
    case 'checked_in': 
    case 'da_tiep_nhan': 
      label = 'Đã xác nhận'; bgClass = 'bg-primary'; break;
    case 'cho_kham': 
    case 'cho_xet_nghiem': 
    case 'da_den_luot': 
    case 'cho_goi_lai': 
      label = isExam ? 'Chờ khám' : 'Chờ xét nghiệm'; bgClass = 'bg-warning text-dark'; break;
    case 'dang_kham': 
    case 'dang_lay_mau': 
      label = isExam ? 'Đang khám' : 'Đang lấy mẫu'; bgClass = 'bg-warning text-dark'; break;
    case 'da_chi_dinh_xn': 
      label = 'Chỉ định xét nghiệm'; bgClass = 'bg-secondary'; break;
    case 'da_lay_mau': 
    case 'ktv_tiep_nhan': 
    case 'dang_xet_nghiem': 
    case 'cho_duyet_kq': 
      label = 'Chờ kết quả'; bgClass = 'bg-secondary'; break;
    case 'da_co_kq': 
    case 'moi_doc_kq': 
    case 'dang_tu_van_kq': 
      label = 'Đã có kết quả'; bgClass = 'bg-success'; break;
    case 'completed': 
    case 'hoan_tat': 
      label = 'Hoàn thành'; bgClass = 'bg-success'; break;
    default: 
      label = status; bgClass = 'bg-secondary'; break;
  }

  return (
    <span className={`badge ${bgClass} px-3 py-2 rounded-pill fw-medium`}>
      {label}
    </span>
  );
};

export default function LichHenChiTiet() {
  const { id } = useParams();
  const navigate = useNavigate();
  
  const [appointment, setAppointment] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let active = true;

    const fetchDetail = async () => {
      try {
        const data = await getAppointmentDetail(id);
        if (!active) return;

        if (data.error) {
          setError(data.error);
        } else {
          setAppointment(data);
        }
      } catch (err) {
        if (active) {
          setError(getApiErrorMessage(err, 'Không thể tải thông tin chi tiết lịch hẹn.'));
        }
      } finally {
        if (active) setLoading(false);
      }
    };

    fetchDetail();
    return () => { active = false; };
  }, [id]);

  if (loading) {
    return (
      <div className="bg-light min-vh-100 py-5 d-flex justify-content-center align-items-center">
        <div className="text-center text-secondary">
          <div className="spinner-border text-primary mb-3" role="status"></div>
          <h5>Đang tải chi tiết lịch hẹn...</h5>
        </div>
      </div>
    );
  }

  if (error || !appointment) {
    return (
      <div className="bg-light min-vh-100 py-5">
        <div className="container-fluid px-md-5">
          <div className="alert alert-danger shadow-sm">
            <h5 className="alert-heading">Lỗi!</h5>
            <p className="mb-0">{error || 'Không tìm thấy lịch hẹn.'}</p>
          </div>
          <button className="btn btn-outline-secondary mt-3" onClick={() => navigate(-1)}>
            <i className="fa-solid fa-arrow-left me-2"></i> Quay lại
          </button>
        </div>
      </div>
    );
  }

  const appointmentDate = new Date(appointment.date || appointment.appointmentDate);
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const isPast = appointmentDate < today;
  const statusLower = String(appointment.status).toLowerCase();
  const canEdit = !isPast && (statusLower === 'pending' || statusLower === 'confirmed');

  return (
    <div className="bg-light min-vh-100 py-4">
      <div className="container-fluid px-md-5">
        
        {/* ĐÃ CẬP NHẬT: Sửa icon FontAwesome cho nút quay lại */}
        <div className="d-flex justify-content-between align-items-center mb-4">
          <div className="d-flex align-items-center">
            <button className="btn btn-white border shadow-sm rounded-circle me-3" onClick={() => navigate('/lich-hen')}>
              <i className="fa-solid fa-arrow-left"></i>
            </button>
            <div>
              <h3 className="fw-bold mb-1" style={{ color: '#0b63e5' }}>
                Chi tiết lịch hẹn: {appointment.id}
              </h3>
              <p className="text-muted mb-0">Theo dõi thông tin và tiến trình khám/xét nghiệm của bạn.</p>
            </div>
          </div>
          {canEdit && (
            <Link to={`/lich-hen/sua?id=${id}`} className="btn btn-outline-primary rounded-pill px-4 shadow-sm">
              <i className="fa-solid fa-pen me-2"></i> Thay đổi / Hủy lịch
            </Link>
          )}
        </div>

        <div className="row g-4">
          <div className="col-lg-7 col-md-12">
            <div className="card border-0 shadow-sm rounded-3 mb-4">
              <div className="card-header bg-white border-bottom-0 pt-4 pb-0 px-4">
                <h5 className="fw-bold text-dark">Thông tin chung</h5>
              </div>
              <div className="card-body p-4">
                <div className="row mb-3">
                  <div className="col-sm-4 text-muted">Trạng thái hiện tại:</div>
                  {/* ĐÃ CẬP NHẬT: Truyền type vào đây */}
                  <div className="col-sm-8">{renderStatusBadge(appointment.status, appointment.type)}</div>
                </div>
                <hr className="text-muted opacity-25" />
                
                <div className="row mb-3">
                  <div className="col-sm-4 text-muted">Phân loại:</div>
                  <div className="col-sm-8 fw-medium">
                    {appointment.type === 'EXAMINATION' ? (
                      <span className="text-primary">Khám bệnh</span>
                    ) : (
                      <span className="text-success">Xét nghiệm</span>
                    )}
                  </div>
                </div>
                
                <div className="row mb-3">
                  <div className="col-sm-4 text-muted">Ngày thực hiện:</div>
                  <div className="col-sm-8 fw-medium">{formatDate(appointment.date || appointment.appointmentDate)}</div>
                </div>
                
                <div className="row mb-3">
                  <div className="col-sm-4 text-muted">Giờ dự kiến:</div>
                  <div className="col-sm-8 fw-medium">
                    {(appointment.time || appointment.appointmentTime) 
                      ? String(appointment.time || appointment.appointmentTime).substring(0, 5) 
                      : '—'}
                  </div>
                </div>
                
                <div className="row mb-3">
                  <div className="col-sm-4 text-muted">Bệnh nhân:</div>
                  <div className="col-sm-8 fw-medium">{appointment.customerName || '—'}</div>
                </div>
                
                <div className="row mb-3">
                  <div className="col-sm-4 text-muted">Bác sĩ phụ trách:</div>
                  <div className="col-sm-8 fw-medium">{appointment.doctorName || 'Được sắp xếp khi đến nơi'}</div>
                </div>
                
                <div className="row mb-3">
                  <div className="col-sm-4 text-muted">Ghi chú:</div>
                  <div className="col-sm-8 fw-medium">{appointment.note || 'Không có ghi chú'}</div>
                </div>
              </div>
            </div>

            {appointment.testOrders && appointment.testOrders.length > 0 && (
              <div className="card border-0 shadow-sm rounded-3">
                <div className="card-header bg-white border-bottom-0 pt-4 pb-0 px-4">
                  <h5 className="fw-bold text-dark">Chỉ định xét nghiệm đi kèm</h5>
                </div>
                <div className="card-body p-4">
                  {appointment.testOrders.map((order, idx) => (
                    <div key={idx} className="p-3 mb-3 bg-light rounded border">
                      <div className="d-flex justify-content-between mb-2">
                        <strong className="text-primary">Mã phiếu: {order.IDPhieuXetNghiem}</strong>
                        {/* ĐÃ CẬP NHẬT: Truyền cứng type 'TEST' vào phiếu xét nghiệm */}
                        {renderStatusBadge(order.TrangThai, 'TEST')}
                      </div>
                      <div className="text-muted small mb-2">Ngày tạo: {formatDateTime(order.NgayTao)}</div>
                      
                      {order.tests && order.tests.length > 0 ? (
                        <div className="d-flex flex-wrap gap-2 mt-2">
                          {order.tests.map((testName, i) => (
                            <span key={i} className="badge bg-white text-dark border p-2">
                              {testName}
                            </span>
                          ))}
                        </div>
                      ) : (
                        <em className="text-muted small">Chưa có danh sách xét nghiệm</em>
                      )}
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>

          <div className="col-lg-5 col-md-12">
            <div className="card border-0 shadow-sm rounded-3 h-100">
              <div className="card-header bg-white border-bottom-0 pt-4 pb-0 px-4">
                <h5 className="fw-bold text-dark">Lịch sử trạng thái</h5>
              </div>
              <div className="card-body p-4">
                
                {(!appointment.timeline || appointment.timeline.length === 0) ? (
                  <p className="text-muted">Chưa có dữ liệu lịch sử.</p>
                ) : (
                  <div className="position-relative ms-3 mt-2" style={{ borderLeft: '2px solid #dee2e6' }}>
                    {appointment.timeline.map((item, idx) => {
                      const isLatest = idx === appointment.timeline.length - 1;
                      return (
                        <div key={idx} className="position-relative mb-4" style={{ paddingLeft: '1.5rem' }}>
                          <div 
                            className={`position-absolute rounded-circle ${isLatest ? 'bg-primary' : 'bg-secondary'}`} 
                            style={{ 
                              width: '14px', 
                              height: '14px', 
                              left: '-8px', 
                              top: '2px', 
                              border: '2px solid #fff',
                              boxShadow: isLatest ? '0 0 0 3px rgba(11, 99, 229, 0.2)' : 'none'
                            }}
                          ></div>
                          
                          <div className={`small mb-1 ${isLatest ? 'text-primary fw-medium' : 'text-muted'}`}>
                            {formatDateTime(item.time)}
                          </div>
                          
                          <div className={`fw-medium ${isLatest ? 'text-dark' : 'text-secondary'}`}>
                            {item.event}
                          </div>
                        </div>
                      );
                    })}
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}