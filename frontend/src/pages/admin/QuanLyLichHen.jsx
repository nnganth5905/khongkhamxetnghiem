import React, { useState, useEffect } from 'react';
import { getAppointments, checkInAppointment } from '../../services/appointmentService';

export default function QuanLyLichHen() {
    const [appointments, setAppointments] = useState([]);
    const [loading, setLoading] = useState(false);
    
    // States cho bộ lọc
    const [filterDate, setFilterDate] = useState(new Date().toISOString().split('T')[0]);
    const [searchTerm, setSearchTerm] = useState('');
    const [filterStatus, setFilterStatus] = useState('all');

    // States cho phân trang
    const [currentPage, setCurrentPage] = useState(1);
    const recordsPerPage = 10;

    useEffect(() => {
        fetchAppointments();
    }, [filterDate]);

    // Reset về trang 1 khi thay đổi từ khóa tìm kiếm hoặc bộ lọc
    useEffect(() => {
        setCurrentPage(1);
    }, [searchTerm, filterStatus]);

    const fetchAppointments = async () => {
        setLoading(true);
        try {
            const data = await getAppointments({ date: filterDate });
            setAppointments(data || []);
        } catch (error) {
            console.error("Lỗi khi tải lịch hẹn:", error);
        } finally {
            setLoading(false);
        }
    };

    const handleCheckIn = async (maDatLich, type) => {
        if (!window.confirm('Xác nhận khách hàng đã đến và tiến hành Check-in?')) return;
        
        try {
            await checkInAppointment(maDatLich, { type: type });
            alert('Check-in thành công! Bệnh nhân đã được chuyển vào hàng đợi.');
            fetchAppointments(); 
        } catch (error) {
            alert('Lỗi khi Check-in, vui lòng thử lại sau.');
        }
    };

    // Logic xử lý tìm kiếm và lọc dữ liệu
    const filteredAppointments = appointments.filter((apt) => {
        if (filterStatus !== 'all' && apt.status !== filterStatus) {
            return false;
        }
        
        if (searchTerm) {
            const term = searchTerm.toLowerCase();
            const matchId = apt.id?.toLowerCase().includes(term);
            const matchName = apt.customerName?.toLowerCase().includes(term);
            const matchPhone = apt.customerPhone?.includes(term);
            
            if (!matchId && !matchName && !matchPhone) {
                return false;
            }
        }
        
        return true;
    });

    // Logic phân trang
    const totalPages = Math.ceil(filteredAppointments.length / recordsPerPage);
    const indexOfLastRecord = currentPage * recordsPerPage;
    const indexOfFirstRecord = indexOfLastRecord - recordsPerPage;
    const currentRecords = filteredAppointments.slice(indexOfFirstRecord, indexOfLastRecord);

    const handleFirstPage = () => setCurrentPage(1);
    const handlePrevPage = () => setCurrentPage((prev) => Math.max(prev - 1, 1));
    const handleNextPage = () => setCurrentPage((prev) => Math.min(prev + 1, totalPages));
    const handleLastPage = () => setCurrentPage(totalPages);

    return (
        <div className="container-fluid p-4" style={{ backgroundColor: '#f8f9fa', minHeight: '100vh' }}>
            <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-end mb-4">
                <div className="mb-3 mb-md-0">
                    <h2 className="fw-bold mb-1" style={{ color: 'var(--primary, #0d6efd)' }}>Quản lý lịch hẹn</h2>
                    <p className="text-secondary mb-0">Xem danh sách đặt lịch và tiến hành check-in khi khách hàng tới cơ sở.</p>
                </div>
                <div className="d-flex gap-2">
                    <input 
                        type="date" 
                        className="form-control" 
                        value={filterDate}
                        onChange={(e) => setFilterDate(e.target.value)}
                        style={{ width: '200px' }}
                    />
                    <button className="btn btn-outline-primary" onClick={fetchAppointments} title="Tải lại dữ liệu">
                        <i className="fa-solid fa-rotate-right"></i>
                    </button>
                </div>
            </div>

            <div className="card shadow-sm border-0 rounded-4">
                <div className="card-body p-4">
                    <div className="row mb-4">
                        <div className="col-md-6 col-lg-5 mb-3 mb-md-0">
                            <div className="input-group">
                                <span className="input-group-text bg-white border-end-0">
                                    <i className="fa-solid fa-magnifying-glass text-muted"></i>
                                </span>
                                <input 
                                    type="text" 
                                    className="form-control border-start-0 ps-0" 
                                    placeholder="Tìm mã đặt lịch, tên khách hàng, SĐT..." 
                                    value={searchTerm}
                                    onChange={(e) => setSearchTerm(e.target.value)}
                                />
                            </div>
                        </div>
                        <div className="col-md-4 col-lg-3">
                            <select 
                                className="form-select"
                                value={filterStatus}
                                onChange={(e) => setFilterStatus(e.target.value)}
                            >
                                <option value="all">Tất cả trạng thái</option>
                                <option value="pending">Chờ xác nhận</option>
                                <option value="confirmed">Đã xác nhận</option>
                                <option value="checked_in">Đã check-in</option>
                                <option value="cancelled">Đã hủy</option>
                            </select>
                        </div>
                    </div>

                    <div className="table-responsive mb-3">
                        <table className="table table-borderless table-hover align-middle mb-0">
                            <thead className="border-bottom">
                                <tr>
                                    <th className="py-3">Mã lịch hẹn</th>
                                    <th className="py-3">Thời gian</th>
                                    <th className="py-3">Họ tên</th>
                                    <th className="py-3">Số điện thoại</th>
                                    <th className="py-3">Dịch vụ / Bác sĩ</th>
                                    <th className="py-3">Trạng thái</th>
                                    <th className="py-3 text-end">Thao tác</th>
                                </tr>
                            </thead>
                            <tbody>
                                {loading ? (
                                    <tr><td colSpan="7" className="text-center py-5"><div className="spinner-border text-primary" role="status"></div></td></tr>
                                ) : currentRecords.length === 0 ? (
                                    <tr><td colSpan="7" className="text-center py-5 text-muted">Chưa có lịch hẹn nào phù hợp.</td></tr>
                                ) : (
                                    currentRecords.map((apt) => (
                                        <tr key={apt.id} className="border-bottom">
                                            <td className="fw-semibold text-primary">{apt.id}</td>
                                            <td>
                                                <div className="fw-bold">{apt.appointmentTime}</div>
                                            </td>
                                            <td>{apt.customerName}</td>
                                            <td>{apt.customerPhone}</td>
                                            <td>
                                                <div className="mb-1">
                                                    <span className={`badge ${apt.type === 'EXAMINATION' ? 'bg-info text-dark' : 'bg-secondary'}`}>
                                                        {apt.type === 'EXAMINATION' ? 'Khám bệnh' : 'Xét nghiệm'}
                                                    </span>
                                                </div>
                                                <small className="text-muted">{apt.doctorName}</small>
                                            </td>
                                            <td>
                                                {apt.status === 'pending' && <span className="badge bg-warning text-dark px-2 py-1">Chờ xác nhận</span>}

                                                {apt.status === 'checked_in' && <span className="badge bg-success px-2 py-1">Đã check-in</span>}
                                                {apt.status === 'cancelled' && <span className="badge bg-danger px-2 py-1">Đã hủy</span>}
                                                {apt.status === 'huy' && <span className="badge bg-danger px-2 py-1">Đã hủy</span>}
                                            </td>
                                            <td className="text-end">
    {(apt.status === 'pending' || apt.status === 'confirmed') && (
        <button 
            className="btn btn-outline-success rounded-pill px-2 py-1"
            style={{ fontSize: '0.75rem', lineHeight: '1.2' }}
            onClick={() => handleCheckIn(apt.id, apt.type)}
        >
            <i className="fa-solid fa-check me-1"></i> Check-in
        </button>
    )}
</td>
                                        </tr>
                                    ))
                                )}
                            </tbody>
                        </table>
                    </div>

                    {/* Điều hướng phân trang */}
                    {totalPages > 0 && (
                        <div className="d-flex justify-content-between align-items-center pt-3 border-top">
                            <span className="text-muted small">
                                Hiển thị {indexOfFirstRecord + 1} đến {Math.min(indexOfLastRecord, filteredAppointments.length)} trong tổng số {filteredAppointments.length} bản ghi
                            </span>
                            <nav>
                                <ul className="pagination pagination-sm mb-0">
                                    <li className={`page-item ${currentPage === 1 ? 'disabled' : ''}`}>
                                        <button className="page-link" onClick={handleFirstPage} title="Về đầu">
                                            <i className="fa-solid fa-angles-left"></i>
                                        </button>
                                    </li>
                                    <li className={`page-item ${currentPage === 1 ? 'disabled' : ''}`}>
                                        <button className="page-link" onClick={handlePrevPage} title="Trang trước">
                                            <i className="fa-solid fa-angle-left"></i>
                                        </button>
                                    </li>
                                    <li className="page-item disabled">
                                        <span className="page-link text-dark fw-semibold">
                                            Trang {currentPage} / {totalPages}
                                        </span>
                                    </li>
                                    <li className={`page-item ${currentPage === totalPages ? 'disabled' : ''}`}>
                                        <button className="page-link" onClick={handleNextPage} title="Trang sau">
                                            <i className="fa-solid fa-angle-right"></i>
                                        </button>
                                    </li>
                                    <li className={`page-item ${currentPage === totalPages ? 'disabled' : ''}`}>
                                        <button className="page-link" onClick={handleLastPage} title="Về cuối">
                                            <i className="fa-solid fa-angles-right"></i>
                                        </button>
                                    </li>
                                </ul>
                            </nav>
                        </div>
                    )}

                </div>
            </div>
        </div>
    );
}