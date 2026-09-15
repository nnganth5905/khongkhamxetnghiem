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

const normalizeTest = (item = {}) => ({
  id:
    item.id ??
    item.idXetNghiem ??
    item.IDXetNghiem,

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
    'Thông tin đang được cập nhật.',

  image:
    item.image ??
    item.hinhAnh ??
    item.HinhAnh ??
    'https://placehold.co/900x520/eaf2ff/0360d9?text=Bio+Medic',

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

export default function TestFinal() {
  const [tests, setTests] = useState([]);

  const [keyword, setKeyword] = useState('');
  const [category, setCategory] = useState('');

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let active = true;

    const loadTests = async () => {
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

        setTests(
          list.map(normalizeTest)
        );
      } catch (err) {
        if (active) {
          setError(
            getApiErrorMessage(
              err,
              'Không thể tải danh mục xét nghiệm.'
            )
          );
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    };

    loadTests();

    return () => {
      active = false;
    };
  }, []);

  const categories =
    useMemo(() => {
      return [
        ...new Set(
          tests
            .map(
              (item) => item.category
            )
            .filter(Boolean)
        ),
      ].sort();
    }, [tests]);

  const filteredTests =
    useMemo(() => {
      const q =
        keyword
          .trim()
          .toLowerCase();

      return tests.filter((item) => {
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
      tests,
      keyword,
      category,
    ]);

  return (
    <div className="bg-light min-vh-100 py-5">
      <div
        className="container"
        style={{
          maxWidth: 1180,
        }}
      >
        <div className="text-center mb-5">
          <h1
            className="fw-bold"
            style={{
              color:
                'var(--primary)',
            }}
          >
            Danh mục xét nghiệm
          </h1>

          <p
            className="text-secondary mx-auto mb-0"
            style={{
              maxWidth: 760,
            }}
          >
            Tìm kiếm xét nghiệm, xem thông tin chi tiết và đặt lịch trực tuyến tại Bio Medic Center.
          </p>
        </div>

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
                    placeholder="Tìm tên xét nghiệm, nhóm xét nghiệm..."
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
          <Loading text="Đang tải danh mục xét nghiệm..." />
        ) : (
          <>
            <div className="d-flex justify-content-between align-items-center mb-3">
              <div className="text-secondary">
                Tìm thấy{' '}
                <strong>
                  {filteredTests.length}
                </strong>{' '}
                xét nghiệm
              </div>
            </div>

            <div className="row g-4">
              {filteredTests.map((item, index) => (
                <div
                  className="col-md-6 col-lg-4"
                  key={
                    item.id ??
                    index
                  }
                >
                  <div className="card h-100 border-0 shadow-sm rounded-4 overflow-hidden">
                    <img
                      src={item.image}
                      alt={item.name}
                      className="card-img-top"
                      style={{
                        height:
                          200,
                        objectFit:
                          'cover',
                      }}
                    />

                    <div className="card-body p-4 d-flex flex-column">
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
                        {item.turnaround}
                      </div>

                      <div
                        className="fw-bold fs-5 mb-3"
                        style={{
                          color:
                            '#ff6a00',
                        }}
                      >
                        {formatVnd(
                          item.price
                        )}
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

              {filteredTests.length === 0 && (
                <div className="col-12">
                  <div className="alert alert-light border text-center">
                    Không có xét nghiệm phù hợp.
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