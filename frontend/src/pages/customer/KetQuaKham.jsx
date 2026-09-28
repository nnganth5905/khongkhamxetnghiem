import React, { useEffect, useMemo, useState } from 'react';
import {
  getMyExaminations,
  getExaminationErrorMessage,
} from '../../services/examinationService';

export default function KetQuaKham() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [search, setSearch] = useState('');

  const loadData = async () => {
    try {
      setLoading(true);
      setError('');

      const data = await getMyExaminations();

      setItems(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(
        getExaminationErrorMessage(
          err,
          'Không thể tải kết quả khám.'
        )
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const filtered = useMemo(() => {
    const keyword = search.trim().toLowerCase();

    if (!keyword) return items;

    return items.filter((item) => {
      return (
        String(item.doctorName || '')
          .toLowerCase()
          .includes(keyword) ||
        String(item.diagnosis || '')
          .toLowerCase()
          .includes(keyword) ||
        String(item.conclusion || '')
          .toLowerCase()
          .includes(keyword)
      );
    });
  }, [items, search]);

  const formatDate = (value) => {
    if (!value) return '—';

    const date = new Date(value);

    if (Number.isNaN(date.getTime())) return value;

    return date.toLocaleString('vi-VN');
  };

  if (loading) {
    return (
      <div className="py-5 text-center">
        Đang tải kết quả khám...
      </div>
    );
  }

  return (
    <div className="container-fluid py-4">
      <div className="mb-4">
        <h1 className="fw-bold text-primary">
          Kết quả khám
        </h1>

        <p className="text-secondary">
          Xem chẩn đoán, kết luận và hướng điều trị của bác sĩ.
        </p>
      </div>

      {error && (
        <div className="alert alert-danger">
          {error}
        </div>
      )}

      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body p-4">
          <div className="mb-4">
            <input
              type="text"
              className="form-control"
              placeholder="Tìm theo bác sĩ, chẩn đoán, kết luận..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>

          {filtered.length === 0 ? (
            <div className="text-center text-secondary py-5">
              Chưa có kết quả khám.
            </div>
          ) : (
            <div className="d-flex flex-column gap-4">
              {filtered.map((item) => (
                <div
                  key={item.id}
                  className="border rounded-4 p-4"
                >
                  <div className="d-flex flex-wrap justify-content-between gap-3 mb-3">
                    <div>
                      <h5 className="fw-bold mb-1">
                        Kết quả khám #{item.id}
                      </h5>

                      <div className="text-secondary">
                        {formatDate(item.examinationDate)}
                      </div>
                    </div>

                    <span className="badge bg-success align-self-start">
                      Hoàn tất
                    </span>
                  </div>

                  <div className="row g-4">
                    <div className="col-md-6">
                      <div className="text-secondary small">
                        Bác sĩ
                      </div>
                      <div className="fw-semibold">
                        {item.doctorName || '—'}
                      </div>
                    </div>

                    <div className="col-md-6">
                      <div className="text-secondary small">
                        Mã lượt khám
                      </div>
                      <div className="fw-semibold">
                        {item.visitId || '—'}
                      </div>
                    </div>

                    <div className="col-md-6">
                      <div className="text-secondary small">
                        Triệu chứng
                      </div>
                      <div>
                        {item.symptoms || '—'}
                      </div>
                    </div>

                    <div className="col-md-6">
                      <div className="text-secondary small">
                        Tiền sử bệnh
                      </div>
                      <div>
                        {item.medicalHistory || '—'}
                      </div>
                    </div>

                    <div className="col-md-6">
                      <div className="text-secondary small">
                        Chẩn đoán
                      </div>
                      <div className="fw-semibold">
                        {item.diagnosis || '—'}
                      </div>
                    </div>

                    <div className="col-md-6">
                      <div className="text-secondary small">
                        Kết luận
                      </div>
                      <div className="fw-semibold">
                        {item.conclusion || '—'}
                      </div>
                    </div>

                    <div className="col-12">
                      <div className="text-secondary small">
                        Hướng điều trị
                      </div>

                      <div>
                        {item.treatment || '—'}
                      </div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}