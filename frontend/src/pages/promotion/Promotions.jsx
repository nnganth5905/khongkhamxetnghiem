import React, { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import { getPromotions } from '../../services/newsService';
import { getApiErrorMessage } from '../../services/api';

const FALLBACK_PROMOTIONS = [
  {
    id: 'adn',
    title: 'Xét nghiệm ADN huyết thống - Ưu đãi 20%',
    summary:
      'Ưu đãi chi phí cho khách hàng đăng ký xét nghiệm ADN huyết thống tại Bio Medic Center.',
    image:
      'https://placehold.co/900x520/eaf2ff/0360d9?text=Xet+nghiem+ADN',
    startDate: '01/09/2026',
    endDate: '30/09/2026',
    route: '/tin-tuc/adn',
  },
  {
    id: 'covid',
    title: 'Ưu đãi xét nghiệm virus đường hô hấp',
    summary:
      'Chương trình hỗ trợ chi phí xét nghiệm cho khách hàng có nhu cầu kiểm tra các tác nhân đường hô hấp.',
    image:
      'https://placehold.co/900x520/eaf2ff/0360d9?text=Virus',
    startDate: '01/09/2026',
    endDate: '30/09/2026',
    route: '/tin-tuc/covid',
  },
  {
    id: 'tim-mach',
    title: 'Giảm 40% gói kiểm tra tim mạch',
    summary:
      'Chương trình ưu đãi dành cho gói xét nghiệm và kiểm tra các chỉ số nguy cơ tim mạch.',
    image:
      'https://placehold.co/900x520/eaf2ff/0360d9?text=Tim+mach',
    startDate: '01/09/2026',
    endDate: '30/09/2026',
    route: '/tin-tuc/tim-mach',
  },
  {
    id: 'voucher',
    title: 'Tặng voucher khi đặt lịch online',
    summary:
      'Khách hàng đặt lịch trực tuyến nhận voucher ưu đãi cho lần sử dụng dịch vụ tiếp theo.',
    image:
      'https://placehold.co/900x520/eaf2ff/0360d9?text=Voucher',
    startDate: '01/09/2026',
    endDate: '30/09/2026',
    route: '/tin-tuc/voucher',
  },
];

const normalizePromotion = (item = {}, index = 0) => ({
  id:
    item.id ??
    item.idKhuyenMai ??
    item.IDKhuyenMai ??
    index,

  title:
    item.title ??
    item.tieuDe ??
    item.TieuDe ??
    'Chương trình ưu đãi',

  summary:
    item.summary ??
    item.description ??
    item.moTa ??
    item.MoTa ??
    '',

  image:
    item.image ??
    item.hinhAnh ??
    item.HinhAnh ??
    'https://placehold.co/900x520/eaf2ff/0360d9?text=Bio+Medic',

  startDate:
    item.startDate ??
    item.ngayBatDau ??
    item.NgayBatDau ??
    '',

  endDate:
    item.endDate ??
    item.ngayKetThuc ??
    item.NgayKetThuc ??
    '',

  route:
    item.route ??
    item.url ??
    `/khuyen-mai/${
      item.id ??
      item.idKhuyenMai ??
      item.IDKhuyenMai ??
      index
    }`,
});

export default function Promotions() {
  const [promotions, setPromotions] = useState(FALLBACK_PROMOTIONS);
  const [keyword, setKeyword] = useState('');

  const [loading, setLoading] = useState(true);

  const [message, setMessage] = useState({
    type: '',
    text: '',
  });

  useEffect(() => {
    let active = true;

    const loadPromotions = async () => {
      try {
        const data = await getPromotions();

        if (!active) {
          return;
        }

        const list = Array.isArray(data)
          ? data
          : data?.content ||
            data?.items ||
            data?.data ||
            [];

        if (list.length > 0) {
          setPromotions(
            list.map(normalizePromotion)
          );
        }
      } catch (err) {
        if (active) {
          setMessage({
            type: 'warning',
            text: getApiErrorMessage(
              err,
              'Chưa tải được dữ liệu ưu đãi từ backend. Hệ thống đang hiển thị nội dung mẫu.'
            ),
          });
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    };

    loadPromotions();

    return () => {
      active = false;
    };
  }, []);

  const filteredPromotions = useMemo(() => {
    const q = keyword.trim().toLowerCase();

    if (!q) {
      return promotions;
    }

    return promotions.filter((item) =>
      `${item.title} ${item.summary}`
        .toLowerCase()
        .includes(q)
    );
  }, [promotions, keyword]);

  return (
    <div className="bg-light min-vh-100 py-5">
      <div
        className="container"
        style={{
          maxWidth: 1150,
        }}
      >
        <div className="text-center mb-5">
          <h1
            className="fw-bold"
            style={{
              color: 'var(--primary)',
            }}
          >
            Chương trình ưu đãi
          </h1>

          <p
            className="text-secondary mx-auto mb-0"
            style={{
              maxWidth: 760,
            }}
          >
            Cập nhật các chương trình ưu đãi và quyền lợi dành cho khách hàng Bio Medic Center.
          </p>
        </div>

        {message.text && (
          <Notification
            type={message.type}
            message={message.text}
            onClose={() =>
              setMessage({
                type: '',
                text: '',
              })
            }
          />
        )}

        <div className="card border-0 shadow-sm rounded-4 mb-4">
          <div className="card-body p-4">
            <div className="input-group">
              <span className="input-group-text bg-white">
                <i className="fa-solid fa-magnifying-glass text-secondary" />
              </span>

              <input
                type="search"
                className="form-control"
                placeholder="Tìm chương trình ưu đãi..."
                value={keyword}
                onChange={(e) =>
                  setKeyword(
                    e.target.value
                  )
                }
              />
            </div>
          </div>
        </div>

        {loading ? (
          <Loading text="Đang tải chương trình ưu đãi..." />
        ) : (
          <div className="row g-4">
            {filteredPromotions.map((item, index) => (
              <div
                className="col-md-6"
                key={item.id ?? index}
              >
                <div className="card border-0 shadow-sm rounded-4 h-100 overflow-hidden">
                  <img
                    src={item.image}
                    alt={item.title}
                    className="card-img-top"
                    style={{
                      height: 260,
                      objectFit: 'cover',
                    }}
                  />

                  <div className="card-body p-4 d-flex flex-column">
                    {(item.startDate ||
                      item.endDate) && (
                      <div className="small text-secondary mb-2">
                        <i className="fa-regular fa-calendar me-2" />

                        {item.startDate || '—'}

                        {item.endDate
                          ? ` - ${item.endDate}`
                          : ''}
                      </div>
                    )}

                    <h4 className="fw-bold">
                      {item.title}
                    </h4>

                    <p
                      className="text-secondary flex-grow-1"
                      style={{
                        lineHeight: 1.7,
                      }}
                    >
                      {item.summary}
                    </p>

                    <Link
                      to={item.route}
                      className="btn btn-primary align-self-start"
                    >
                      Xem chi tiết
                      <i className="fa-solid fa-arrow-right ms-2" />
                    </Link>
                  </div>
                </div>
              </div>
            ))}

            {filteredPromotions.length === 0 && (
              <div className="col-12">
                <div className="alert alert-light border text-center">
                  Không tìm thấy chương trình phù hợp.
                </div>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}