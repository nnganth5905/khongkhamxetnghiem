import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getApiErrorMessage, getMyAppointments } from '../../services/appointmentService';

const formatDate = (dateStr) => {
  if (!dateStr) return '—';
  const cleanDate = String(dateStr).substring(0, 10);
  const parts = cleanDate.split('-');
  if (parts.length === 3) return `${parts[2]}/${parts[1]}/${parts[0]}`;
  
  const d = new Date(dateStr);
  if (Number.isNaN(d.getTime())) return dateStr;
  return `${String(d.getDate()).padStart(2, '0')}/${String(d.getMonth() + 1).padStart(2, '0')}/${d.getFullYear()}`;
};

const renderStatusBadge = (status) => {
  const value = String(status || '').toLowerCase();
  
  if (['cancelled', 'huy'].includes(value)) {
    return <span className="badge bg-danger px-3 py-2 rounded-pill">Đã hủy</span>;
  }
  if (['no_show', 'bo_luot'].includes(value)) {
    return <span className="badge bg-dark px-3 py-2 rounded-pill">Bỏ lượt</span>;
  }

  let label = status;
  switch (value) {
    case 'pending': 
      label = 'Đã đặt lịch'; 
      break;
    case 'confirmed': 
    case 'checked_in': 
    case 'da_tiep_nhan': 
      label = 'Đã xác nhận'; 
      break;
    case 'cho_kham': 
    case 'cho_xet_nghiem': 
    case 'da_den_luot': 
    case 'cho_goi_lai': 
      label = 'Chờ khám / Chờ XN'; 
      break;
    case 'dang_kham': 
    case 'dang_lay_mau': 
      label = 'Đang khám / Lấy mẫu'; 
      break;
    case 'da_chi_dinh_xn': 
      label = 'Chỉ định xét nghiệm'; 
      break;
    case 'da_lay_mau': 
    case 'ktv_tiep_nhan': 
    case 'dang_xet_nghiem': 
    case 'cho_duyet_kq': 
      label = 'Chờ kết quả'; 
      break;
    case 'da_co_kq': 
    case 'moi_doc_kq': 
    case 'dang_tu_van_kq': 
      label = 'Đã có kết quả'; 
      break;
    case 'completed': 
    case 'hoan_tat': 
      label = 'Hoàn thành'; 
      break;
    default:
      label = status;
      break;
  }

  return (
    <span 
      className="badge px-3 py-2 rounded-pill fw-medium" 
      style={{ backgroundColor: '#e6f4ea', color: '#137333', border: '1px solid #ceead6' }}
    >
      {label}
    </span>
  );
};

export default function MyAppointments({ initialAppointments = null }) {
  const [appointments, setAppointments] = useState(initialAppointments || []);
  const [loading, setLoading] = useState(!initialAppointments);
  const [error, setError] = useState('');
  
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('ALL');
  
  // States cho khoảng thời gian
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');

  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;

  useEffect(() => {
    if (initialAppointments) return;
    let active = true;

    const loadAppointments = async () => {
      try {
        const data = await getMyAppointments();
        if (!active) return;
        const items = Array.isArray(data) ? data : data?.appointments || data?.items || [];
        setAppointments(items);
      } catch (err) {
        if (active) setError(getApiErrorMessage(err, 'Không thể tải danh sách lịch hẹn.'));
      } finally {
        if (active) setLoading(false);
      }
    };

    loadAppointments();
    return () => { active = false; };
  }, [initialAppointments]);

  // Reset về trang 1 mỗi khi thay đổi bất kỳ bộ lọc nào
  useEffect(() => {
    setCurrentPage(1);
  }, [searchTerm, statusFilter, startDate, endDate]);

  const filteredAppointments = appointments.filter((item) => {
    // 1. Lọc theo trạng thái
    if (statusFilter !== 'ALL') {
      const itemStatus = String(item.status).toLowerCase();
      if (statusFilter === 'pending' && itemStatus !== 'pending') return false;
      if (statusFilter === 'confirmed' && !['confirmed', 'checked_in', 'da_tiep_nhan'].includes(itemStatus)) return false;
      if (statusFilter === 'waiting' && !['cho_kham', 'cho_xet_nghiem', 'da_den_luot', 'cho_goi_lai'].includes(itemStatus)) return false;
      if (statusFilter === 'in_progress' && !['dang_kham', 'dang_lay_mau', 'da_chi_dinh_xn'].includes(itemStatus)) return false;
      if (statusFilter === 'testing' && !['da_lay_mau', 'ktv_tiep_nhan', 'dang_xet_nghiem', 'cho_duyet_kq'].includes(itemStatus)) return false;
      if (statusFilter === 'has_result' && !['da_co_kq', 'moi_doc_kq', 'dang_tu_van_kq'].includes(itemStatus)) return false;
      if (statusFilter === 'completed' && !['completed', 'hoan_tat'].includes(itemStatus)) return false;
      if (statusFilter === 'cancelled' && !['cancelled', 'huy', 'no_show', 'bo_luot'].includes(itemStatus)) return false;
    }

    // 2. Lọc theo khoảng thời gian
    if (startDate !== '') {
      if (!item.appointmentDate || item.appointmentDate < startDate) return false;
    }
    if (endDate !== '') {
      if (!item.appointmentDate || item.appointmentDate > endDate) return false;
    }

    // 3. Lọc theo text
    if (searchTerm.trim() !== '') {
      const term = searchTerm.toLowerCase();
      const id = (item.id || '').toLowerCase();
      const customer = (item.customerName || '').toLowerCase();
      const doctor = (item.doctorName || '').toLowerCase();
      const typeStr = item.type === 'EXAMINATION' ? 'khám bệnh' : 'xét nghiệm';

      if (!id.includes(term) && !customer.includes(term) && !doctor.includes(term) && !typeStr.includes(term)) {
        return false;
      }
    }
    return true;
  });

  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentItems = filteredAppointments.slice(indexOfFirstItem, indexOfLastItem);
  const totalPages = Math.ceil(filteredAppointments.length / itemsPerPage);

  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
  };

  return (
    <div className="bg-light min-vh-100 py-4">
      <div className="container-fluid px-md-5">
        <div className="d-flex justify-content-between align-items-center mb-4">
          <div>
            <h3 className="fw-bold mb-1" style={{ color: '#0b63e5' }}>Lịch hẹn của tôi</h3>
            <p className="text-muted mb-0">Xem và quản lý các lịch hẹn đã đặt trên hệ thống.</p>
          </div>
        </div>

        {error && <div className="alert alert-danger shadow-sm">{error}</div>}

        <div className="card border-0 shadow-sm rounded-3">
          <div className="card-body p-4">
            
            <div className="row mb-4 g-3">
              <div className="col-lg-4 col-md-6">
                <div className="input-group">
                  <span className="input-group-text bg-white text-muted border-end-0">
                    <span role="img" aria-label="search">🔍</span>
                  </span>
                  <input
                    type="text"
                    className="form-control border-start-0 ps-0"
                    placeholder="Tìm mã, bác sĩ, bệnh nhân..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                  />
                </div>
              </div>

              <div className="col-lg-3 col-md-6">
                <select
                  className="form-select text-secondary"
                  value={statusFilter}
                  onChange={(e) => setStatusFilter(e.target.value)}
                >
                  <option value="ALL">Tất cả trạng thái</option>
                  <option value="pending">Đã đặt lịch</option>
                  <option value="confirmed">Đã xác nhận</option>
                  <option value="waiting">Chờ khám / Chờ XN</option>
                  <option value="in_progress">Đang khám / Lấy mẫu</option>
                  <option value="testing">Chờ kết quả</option>
                  <option value="has_result">Đã có kết quả</option>
                  <option value="completed">Hoàn thành</option>
                  <option value="cancelled">Đã hủy / Bỏ lượt</option>
                </select>
              </div>

              <div className="col-lg-5 col-md-12">
                <div className="input-group">
                  <span className="input-group-text bg-light text-muted">Từ</span>
                  <input
                    type="date"
                    className="form-control text-secondary"
                    value={startDate}
                    onChange={(e) => setStartDate(e.target.value)}
                  />
                  <span className="input-group-text bg-light text-muted border-start-0 border-end-0">đến</span>
                  <input
                    type="date"
                    className="form-control text-secondary"
                    value={endDate}
                    onChange={(e) => setEndDate(e.target.value)}
                  />
                </div>
              </div>
            </div>

            {loading ? (
              <div className="text-center py-5 text-secondary">
                <div className="spinner-border text-primary mb-2" role="status"></div>
                <div>Đang tải lịch hẹn...</div>
              </div>
            ) : filteredAppointments.length === 0 ? (
              <div className="text-center py-5 text-muted">Chưa có lịch hẹn phù hợp.</div>
            ) : (
              <>
                <div className="table-responsive">
                  <table className="table table-hover align-middle mb-0" style={{ fontSize: '0.95rem' }}>
                    <thead className="table-light">
                      <tr>
                        <th className="text-center text-nowrap">#</th>
                        <th className="text-nowrap">Mã Đặt Lịch</th>
                        <th className="text-nowrap">Ngày</th>
                        <th className="text-nowrap">Giờ</th>
                        <th>Phân loại</th>
                        <th>Bệnh nhân</th>
                        <th>Bác sĩ</th>
                        <th className="text-center text-nowrap">Trạng thái</th>
                        <th className="text-center text-nowrap">Thao tác</th>
                      </tr>
                    </thead>
                    <tbody>
                      {currentItems.map((item, index) => (
                        <tr key={item.id || index}>
                          <td className="text-center text-muted">
                            {indexOfFirstItem + index + 1}
                          </td>
                          <td className="text-nowrap"><strong className="text-dark">{item.id}</strong></td>
                          <td className="text-nowrap">{formatDate(item.appointmentDate)}</td>
                          <td className="text-nowrap">{item.appointmentTime ? String(item.appointmentTime).substring(0, 5) : '—'}</td>
                          <td>
                            {item.type === 'EXAMINATION' ? (
                              <span className="text-primary fw-medium">Khám bệnh</span>
                            ) : (
                              <span className="text-success fw-medium">Xét nghiệm</span>
                            )}
                          </td>
                          <td>{item.customerName || '—'}</td>
                          <td>{item.doctorName || '—'}</td>
                          <td className="text-center text-nowrap">{renderStatusBadge(item.status)}</td>
                          <td className="text-center text-nowrap">
                            <Link
                              className="btn btn-sm btn-outline-primary rounded-pill px-3"
                              to={`/lich-hen/${item.id}`}
                            >
                              Chi tiết
                            </Link>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>

                {totalPages > 1 && (
                  <div className="d-flex justify-content-between align-items-center mt-4">
                    <span className="text-muted small">
                      Hiển thị {indexOfFirstItem + 1} - {Math.min(indexOfLastItem, filteredAppointments.length)} trong tổng số {filteredAppointments.length} lịch hẹn
                    </span>
                    <nav>
                      <ul className="pagination pagination-sm mb-0">
                        <li className={`page-item ${currentPage === 1 ? 'disabled' : ''}`}>
                          <button 
                            className="page-link" 
                            onClick={() => handlePageChange(1)}
                            disabled={currentPage === 1}
                            title="Về trang đầu"
                          >
                            &laquo;
                          </button>
                        </li>

                        <li className={`page-item ${currentPage === 1 ? 'disabled' : ''}`}>
                          <button 
                            className="page-link" 
                            onClick={() => handlePageChange(currentPage - 1)}
                            disabled={currentPage === 1}
                            title="Trang trước"
                          >
                            &lsaquo;
                          </button>
                        </li>
                        
                        {Array.from({ length: totalPages }, (_, i) => i + 1).map((page) => (
                          <li key={page} className={`page-item ${currentPage === page ? 'active' : ''}`}>
                            <button className="page-link" onClick={() => handlePageChange(page)}>
                              {page}
                            </button>
                          </li>
                        ))}

                        <li className={`page-item ${currentPage === totalPages ? 'disabled' : ''}`}>
                          <button 
                            className="page-link" 
                            onClick={() => handlePageChange(currentPage + 1)}
                            disabled={currentPage === totalPages}
                            title="Trang sau"
                          >
                            &rsaquo;
                          </button>
                        </li>

                        <li className={`page-item ${currentPage === totalPages ? 'disabled' : ''}`}>
                          <button 
                            className="page-link" 
                            onClick={() => handlePageChange(totalPages)}
                            disabled={currentPage === totalPages}
                            title="Về trang cuối"
                          >
                            &raquo;
                          </button>
                        </li>
                      </ul>
                    </nav>
                  </div>
                )}
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}