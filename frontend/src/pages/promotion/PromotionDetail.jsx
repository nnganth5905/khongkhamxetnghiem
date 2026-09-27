import React, { useEffect, useState } from 'react';
import {
  Link,
  useParams,
} from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import { getPromotion } from '../../services/newsService';
import { getApiErrorMessage } from '../../services/api';

export default function PromotionDetail() {
  const { id } = useParams();

  const queryId =
    new URLSearchParams(
      window.location.search
    ).get('id');

  const promotionId =
    id || queryId;

  const [promotion, setPromotion] = useState(null);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let active = true;

    const loadPromotion = async () => {
      if (!promotionId) {
        setError(
          'Thiếu mã chương trình ưu đãi.'
        );

        setLoading(false);
        return;
      }

      try {
        const data =
          await getPromotion(
            promotionId
          );

        if (active) {
          setPromotion(data);
        }
      } catch (err) {
        if (active) {
          setError(
            getApiErrorMessage(
              err,
              'Không tìm thấy chương trình ưu đãi.'
            )
          );
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    };

    loadPromotion();

    return () => {
      active = false;
    };
  }, [promotionId]);

  if (loading) {
    return (
      <Loading text="Đang tải nội dung ưu đãi..." />
    );
  }

  const title =
    promotion?.title ??
    promotion?.tieuDe ??
    promotion?.TieuDe;

  const image =
    promotion?.image ??
    promotion?.hinhAnh ??
    promotion?.HinhAnh;

  const content =
    promotion?.content ??
    promotion?.noiDung ??
    promotion?.NoiDung ??
    '';

  const startDate =
    promotion?.startDate ??
    promotion?.ngayBatDau ??
    promotion?.NgayBatDau ??
    '';

  const endDate =
    promotion?.endDate ??
    promotion?.ngayKetThuc ??
    promotion?.NgayKetThuc ??
    '';

  return (
    <div className="bg-light min-vh-100 py-5">
      <div
        className="container"
        style={{
          maxWidth: 900,
        }}
      >
        {error && (
          <Notification
            type="danger"
            message={error}
          />
        )}

        {promotion && (
          <article className="card border-0 shadow-sm rounded-4 overflow-hidden">
            {image && (
              <img
                src={image}
                alt={title}
                className="w-100"
                style={{
                  maxHeight: 480,
                  objectFit: 'cover',
                }}
              />
            )}

            <div className="p-4 p-lg-5">
              {(startDate || endDate) && (
                <div className="small text-secondary mb-3">
                  <i className="fa-regular fa-calendar me-2" />

                  {startDate || '—'}

                  {endDate
                    ? ` - ${endDate}`
                    : ''}
                </div>
              )}

              <h1
                className="fw-bold mb-4"
                style={{
                  color: 'var(--primary)',
                }}
              >
                {title}
              </h1>

              <div
                className="promotion-content"
                style={{
                  whiteSpace: 'pre-line',
                  lineHeight: 1.85,
                  fontSize: '17px',
                }}
              >
                {content}
              </div>

              <div className="mt-5 pt-4 border-top d-flex flex-wrap gap-2">
                <Link
                  to="/khuyen-mai"
                  className="btn btn-outline-primary"
                >
                  <i className="fa-solid fa-arrow-left me-2" />
                  Quay lại ưu đãi
                </Link>

                <Link
                  to="/dat-lich-xet-nghiem"
                  className="btn btn-primary"
                >
                  <i className="fa-regular fa-calendar-check me-2" />
                  Đặt lịch
                </Link>
              </div>
            </div>
          </article>
        )}
      </div>
    </div>
  );
}