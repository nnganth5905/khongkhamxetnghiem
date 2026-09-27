import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import { Link } from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import { getTests } from '../../services/testService';
import { getApiErrorMessage } from '../../services/api';

const normalizeService = (item = {}, index = 0) => ({
  id:
    item.id ??
    item.idXetNghiem ??
    item.IDXetNghiem ??
    index,

  name:
    item.name ??
    item.tenXetNghiem ??
    item.TenXetNghiem ??
    'Xét nghiệm',

  category:
    item.category ??
    item.tenLoai ??
    item.TenLoai ??
    'Khác',

  description:
    item.description ??
    item.moTa ??
    item.MoTa ??
    'Thông tin dịch vụ đang được cập nhật.',

  image:
    item.image ??
    item.hinhAnh ??
    item.HinhAnh ??
    'https://placehold.co/900x550/eaf2ff/0360d9?text=Bio+Medic',

  price:
    item.price ??
    item.gia ??
    item.Gia ??
    null,

  turnaround:
    item.turnaround ??
    item.thoiGianTraKetQua ??
    item.ThoiGianTraKetQua ??
    'Theo quy trình',
});

const formatVnd = (value) => {
  if (
    value === null ||
    value === undefined ||
    value === ''
  ) {
    return 'Liên hệ';
  }

  const number = Number(value);

  if (!Number.isFinite(number)) {
    return String(value);
  }

  return (
    number.toLocaleString('vi-VN') +
    ' đ'
  );
};

export default function Services() {
  const [services, setServices] = useState([]);

  const [keyword, setKeyword] = useState('');
  const [category, setCategory] = useState('');

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let active = true;

    const loadServices = async () => {
      try {
        const data =
          await getTests();

        if (!active) {
          return;
        }

        const list = Array.isArray(data)
          ? data
          : data?.content ||
            data?.items ||
            data?.data ||
            [];

        setServices(
          list.map(normalizeService)
        );
      } catch (err) {
        if (active) {
          setError(
            getApiErrorMessage(
              err,
              'Không thể tải danh sách dịch vụ xét nghiệm.'
            )
          );
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    };

    loadServices();

    return () => {
      active = false;
    };
  }, []);

  const categories = useMemo(() => {
    return [
      ...new Set(
        services
          .map(
            (item) => item.category
          )
          .filter(Boolean)
      ),
    ].sort();
  }, [services]);

  const filteredServices =
    useMemo(() => {
      const q =
        keyword
          .trim()
          .toLowerCase();

      return services.filter((item) => {
        const matchesKeyword =
          !q ||
          `${item.name} ${item.category} ${item.description}`
            .toLowerCase()
            .includes(q);

        const matchesCategory =
          !category ||
          item.category === category;

        return (
          matchesKeyword &&
          matchesCategory
        );
      });
    }, [
      services,
      keyword,
      category,
    ]);

  return (
    <div className="bg-light min-vh-100">
      <section className="py-5 bg-white">
        <div
          className="container text-center"
          style={{
            maxWidth: 900,
          }}
        >
          <h1
            className="fw-bold mb-3"
            style={{
              color: 'var(--primary)',
            }}
          >
            Dịch vụ xét nghiệm
          </h1>

          <p
            className="text-secondary mb-0"
            style={{
              lineHeight: 1.8,
            }}
          >
            Bio Medic Center cung cấp các nhóm xét nghiệm từ cơ bản đến chuyên sâu,
            hỗ trợ người bệnh tra cứu thông tin và đặt lịch thuận tiện.
          </p>
        </div>
      </section>

      <div
        className="container py-5"
        style={{
          maxWidth: 1180,
        }}
      >
        {error && (
          <Notification
            type="danger"
            message={error}
            onClose={() => setError('')}
          />
        )}

        <div className="card border-0 shadow-sm rounded-4 mb-4">
          <div className="card-body p-4">
            <div className="row g-3">
              <div className="col-lg-8">
                <div className="input-group">
                  <span className="input-group-text bg-white">
                    <i className="fa-solid fa-magnifying-glass text-secondary" />
                  </span>

                  <input
                    type="search"
                    className="form-control"
                    placeholder="Tìm dịch vụ xét nghiệm..."
                    value={keyword}
                    onChange={(e) =>
                      setKeyword(
                        e.target.value
                      )
                    }
                  />
                </div>
              </div>

              <div className="col-lg-4">
                <select
                  className="form-select"
                  value={category}
                  onChange={(e) =>
                    setCategory(
                      e.target.value
                    )
                  }
                >
                  <option value="">
                    Tất cả nhóm xét nghiệm
                  </option>

                  {categories.map(
                    (name) => (
                      <option
                        key={name}
                        value={name}
                      >
                        {name}
                      </option>
                    )
                  )}
                </select>
              </div>
            </div>
          </div>
        </div>

        {loading ? (
          <Loading text="Đang tải dịch vụ xét nghiệm..." />
        ) : (
          <>
            <div className="d-flex justify-content-between align-items-center mb-3">
              <span className="text-secondary">
                Có{' '}
                <strong>
                  {filteredServices.length}
                </strong>{' '}
                dịch vụ
              </span>
            </div>

            <div className="row g-4">
              {filteredServices.map((item, index) => (
                <div
                  className="col-md-6 col-lg-4"
                  key={item.id ?? index}
                >
                  <div className="card service h-100 overflow-hidden">
                    <img
                      src={item.image}
                      alt={item.name}
                      className="card-img-top"
                    />

                    <div className="card-body d-flex flex-column p-4">
                      <div className="small text-primary fw-semibold mb-1">
                        {item.category}
                      </div>

                      <h5 className="fw-bold">
                        {item.name}
                      </h5>

                      <p className="text-secondary small flex-grow-1">
                        {item.description}
                      </p>

                      <div className="small text-secondary mb-2">
                        <i className="fa-regular fa-clock me-2" />
                        Trả kết quả: {item.turnaround}
                      </div>

                      <div className="price mb-3">
                        <span className="now">
                          {formatVnd(
                            item.price
                          )}
                        </span>
                      </div>

                      <div className="d-flex gap-2">
                        <Link
                          to={`/xet-nghiem/${item.id}`}
                          className="btn btn-outline-primary flex-fill"
                        >
                          Chi tiết
                        </Link>

                        <Link
                          to={`/dat-lich-xet-nghiem?test=${encodeURIComponent(
                            item.id ?? ''
                          )}`}
                          className="btn btn-primary flex-fill"
                        >
                          Đặt lịch
                        </Link>
                      </div>
                    </div>
                  </div>
                </div>
              ))}

              {filteredServices.length === 0 && (
                <div className="col-12">
                  <div className="alert alert-light border text-center">
                    Không có dịch vụ phù hợp.
                  </div>
                </div>
              )}
            </div>
          </>
        )}
      </div>
    </div>
  );
}